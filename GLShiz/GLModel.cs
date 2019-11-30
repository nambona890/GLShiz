using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLShiz
{
    class GLModel : IDisposable
    {
        public GLMesh[] meshes;
        bool disposed = false;

        public GLModel(string[] meshPaths, Texture[] texPaths)
        {
            meshes = new GLMesh[meshPaths.Length];
            for(int i=0;i<meshes.Length;i++)
            {
                meshes[i] = new GLMesh(meshPaths[i], texPaths[i]);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                for (int i = 0; i < meshes.Length; i++)
                {
                    meshes[i].Dispose();
                }
            }
            disposed = true;
        }
    }
}
