using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Sober.Scene
{
    public class Transform
    {
        //local transform => position, location, scale
        private Vector2 _localposition = Vector2.Zero;
        private float _localRotation = 0f;
        private Vector2 _localScale = Vector2.One;

        //Hierarchy
        private Transform? _parent;
        private readonly List<Transform> _child = new();

        //Dirty flag => whenever things change
        private bool _dirty = true;
        private Matrix4 _localMatrix = Matrix4.Identity;
        private Matrix4 _worldMatrix = Matrix4.Identity;

        public void Detach(bool keepWorld = true) => SetParent(null, keepWorld);

        private void MarkDirty()
        {
            if (_dirty) return;
            _dirty = true;
            foreach (var child in _child)
                child.MarkDirty();
        }

        private void RecalculateIfNeeded()
        {
            if (!_dirty) return;
            _localMatrix = Matrix4.CreateScale(_localScale.X, _localScale.Y, 1f) *Matrix4.CreateRotationZ(_localRotation) *Matrix4.CreateTranslation(_localposition.X, _localposition.Y, 0f);
                Matrix4 parentWorld = _parent?.WorldMatrix ?? Matrix4.Identity;
               _worldMatrix = parentWorld * _localMatrix;

            _dirty = false;
        }

        //helper => breaks a 2D transform matrix into position, rotation, and scale.
        private static void Decompose2D(Matrix4 m, out Vector2 pos, out float rot, out Vector2 scale)
        {
            pos = new Vector2(m.M41, m.M42);

            // Get X & Y directions
            Vector2 xAxis = new Vector2(m.M11, m.M12);
            Vector2 yAxis = new Vector2(m.M21, m.M22);
            float sx = xAxis.Length;
            float sy = yAxis.Length;

            // Avoid division by zero
            sx = sx == 0 ? 1 : sx;
            sy = sy == 0 ? 1 : sy;

            //scale is encoded in the length of the axes (must extract before rotation)
            scale = new Vector2(sx, sy);

            //  direction the object is facing with scale/removes scale  => keeps direction only
            Vector2 xNorm = xAxis / sx;

            //converts that direction into an angle
            rot = MathF.Atan2(xNorm.Y, xNorm.X);
        }

        public Vector2 LocalPosition
        {
            get => _localposition;
            set { _localposition = value; MarkDirty(); }
        }

        public float LocalRotation
        {
            get => _localRotation;
            set { _localRotation = value; MarkDirty(); }
        }

        public Vector2 LocalScale
        {
            get => _localScale;
            set { _localScale = value; MarkDirty(); }
        }

        public Transform? Parent => _parent;
        public IReadOnlyList<Transform> Children => _child;

        public Matrix4 LocalMatrix
        {
            get { RecalculateIfNeeded(); return _localMatrix; }
        }

        public Matrix4 WorldMatrix
        {
            get { RecalculateIfNeeded(); return _worldMatrix; }
        }

        public void SetParent(Transform? newParent, bool keepWorld=true)
        {
            Matrix4 currentWorld = WorldMatrix;
            _parent?._child.Remove(this);
            _parent?._child.Add(this);

            if(keepWorld)
            {
                Matrix4 parentWorld = _parent?.WorldMatrix ?? Matrix4.Identity;
                Matrix4 invParent = parentWorld.Inverted();

                Matrix4 local = invParent * currentWorld;
                Decompose2D(local, out _localposition, out _localRotation, out _localScale);
            }
            MarkDirty();
        }
    }
}
