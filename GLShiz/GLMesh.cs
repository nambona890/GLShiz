using System;
using Tao.FreeGlut;
using OpenGL;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace GLShiz
{
    class GLMesh : IDisposable
    {
        public Texture tex;
        public VBO<Vector3> verts;
        public VBO<Vector2> uvCoords;
        public VBO<uint> tris;
        public VBO<Vector3> normals;
        bool disposed = false;

        public GLMesh(Vector3[] vertsExt, Vector2[] uvExt, uint[] trisExt, Vector3[] normalsExt, Texture texExt)
        {
            verts = new VBO<Vector3>(vertsExt);
            uvCoords = new VBO<Vector2>(uvExt);
            tris = new VBO<uint>(trisExt, BufferTarget.ElementArrayBuffer);
            normals = new VBO<Vector3>(normalsExt);
            tex = texExt;
        }

        public GLMesh(string objPath, Texture texExt)
        {

            tex = texExt;

            List<Vector3> impVer = new List<Vector3>();
            List<uint> impTris = new List<uint>();

            List<Vector2> impUV = new List<Vector2>();
            List<uint> impUVTris = new List<uint>();

            byte[] objBuffer = File.ReadAllBytes(objPath);
            string strObj = Encoding.UTF8.GetString(objBuffer);

            string[] words = strObj.Split('\n');

            foreach(string lines in words)
            {
                string[] values = lines.Split(new Char[] { ' ', '/' });
                switch (values[0])
                {
                    case "v":
                        Vector3 tmpVert = new Vector3(0,0,0);
                        Single.TryParse(values[1], out tmpVert.X);
                        Single.TryParse(values[2], out tmpVert.Y);
                        Single.TryParse(values[3], out tmpVert.Z);
                        impVer.Add(tmpVert);
                        break;
                    case "vt":
                        Vector2 tmpUV = new Vector2(0,0);
                        Single.TryParse(values[1], out tmpUV.X);
                        Single.TryParse(values[2], out tmpUV.Y);
                        impUV.Add(tmpUV);
                        break;
                    case "f":
                        uint tmpTri,tmpUVTri;
                        UInt32.TryParse(values[1], out tmpTri);
                        impTris.Add(tmpTri);
                        UInt32.TryParse(values[3], out tmpTri);
                        impTris.Add(tmpTri);
                        UInt32.TryParse(values[5], out tmpTri);
                        impTris.Add(tmpTri);

                        UInt32.TryParse(values[2], out tmpUVTri);
                        impUVTris.Add(tmpUVTri);
                        UInt32.TryParse(values[4], out tmpUVTri);
                        impUVTris.Add(tmpUVTri);
                        UInt32.TryParse(values[6], out tmpUVTri);
                        impUVTris.Add(tmpUVTri);

                        break;
                }
            }
            Vector3[] tmpVer = new Vector3[impTris.Count];
            Vector3[] tmpNormals = new Vector3[impTris.Count];
            Vector2[] tmpUVs = new Vector2[impTris.Count];
            uint[] tmpTris = new uint[impTris.Count];

            for(int i=0;i<impTris.Count;i++)
            {
                tmpVer[i] = impVer[Convert.ToInt32(impTris[i] - 1)];
                tmpUVs[i] = impUV[Convert.ToInt32(impUVTris[i] - 1)];
                tmpTris[i] = Convert.ToUInt32(i);
            }

            for(int i=0;i<impTris.Count;i+=3)
            {
                Vector3 tmpDirection = Vector3.Cross(tmpVer[i + 1] - tmpVer[i], tmpVer[i + 2] - tmpVer[i]);
                tmpNormals[i] = Vector3.Normalize(tmpDirection);
                tmpNormals[i+1] = Vector3.Normalize(tmpDirection);
                tmpNormals[i+2] = Vector3.Normalize(tmpDirection);
            }

            verts = new VBO<Vector3>(tmpVer);
            tris = new VBO<uint>(tmpTris, BufferTarget.ElementArrayBuffer);
            uvCoords = new VBO<Vector2>(tmpUVs);
            normals = new VBO<Vector3>(tmpNormals);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if(disposed)
            {
                return;
            }
            if (disposing)
            {
                tex.Dispose();
                verts.Dispose();
                uvCoords.Dispose();
                tris.Dispose();
                normals.Dispose();
            }
            disposed = true;
        }
    }
}
