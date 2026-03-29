using Sober.ECS;

namespace Sober.Editor
{
    public sealed class RuntimeEditor
    {
        private readonly World _world;
        private readonly EditorSelection _selection;

        public RuntimeEditor(World world, EditorSelection selection)
        {
            _world = world;
            _selection = selection;
        }

        public void Toggle()
        {
            _selection.Open = !_selection.Open;
        }

        public bool IsOpen()
        {
            return _selection.Open;
        }

        public void Select(int entityId)
        {
            _selection.SelectedEntityId = entityId;
        }

        public int SelectedId ()
          {
            return _selection.SelectedEntityId;
        }

        public World GetWorld()
        {
            return _world;
        }
    }
}
