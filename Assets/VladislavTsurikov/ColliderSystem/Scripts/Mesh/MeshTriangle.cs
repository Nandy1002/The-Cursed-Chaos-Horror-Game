namespace VladislavTsurikov.ColliderSystem.Scripts.Mesh
{
    public struct MeshTriangle
    {
        private int _vIndex0;
        private int _vIndex1;
        private int _vIndex2;

        public int VIndex0 { get { return _vIndex0; } }
        public int VIndex1 { get { return _vIndex1; } }
        public int VIndex2 { get { return _vIndex2; } }

        public MeshTriangle(int vIndex0, int vIndex1, int vIndex2)
        {
            _vIndex0 = vIndex0;
            _vIndex1 = vIndex1;
            _vIndex2 = vIndex2;
        }
    }
}