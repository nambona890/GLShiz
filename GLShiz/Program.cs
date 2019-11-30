using System;
using Tao.FreeGlut;
using OpenGL;
using System.IO;
using System.Text;

namespace GLShiz
{
    class Program
    {
        private const int width = 1280, height = 720;
        private const float rotFactor = 2f;
        private const float movFactor = 3f;
        private static ShaderProgram program;
        private static System.Diagnostics.Stopwatch watch;
        private static Vector3 rotation;
        private static Vector4 rotation2;
        private static Vector3 position;
        private static GLModel model;
        private static bool[] keys;
        private static bool[] arrows;
        static void Main(string[] args)
        {
            keys = new bool[256];
            arrows = new bool[4];
            for (int i = 0; i < 256; i++)
            {
                keys[i] = false;
            }
            for (int i = 0; i < 4; i++)
            {
                arrows[i] = false;
            }
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Glut.glutInitWindowSize(width, height);
            Glut.glutCreateWindow("GLShiz");
            Glut.glutIdleFunc(OnRenderFrame);
            Glut.glutDisplayFunc(OnDisplay);
            Glut.glutCloseFunc(OnClose);
            Glut.glutKeyboardFunc(OnKeyDown);
            Glut.glutKeyboardUpFunc(OnKeyUp);
            Glut.glutSpecialFunc(OnSpecialInputDown);
            Glut.glutSpecialUpFunc(OnSpecialInputUp);
            Gl.Enable(EnableCap.DepthTest);
            program = new ShaderProgram(VertexShader, FragmentShader);

            program.Use();
            program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
            program["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, Vector3.UnitY));
            program["light_direction"].SetValue(new Vector3(0, 0, 1));

            //Vector3[] cube = new Vector3[]
            //{
            //    new Vector3(-1,-1,1),new Vector3(-1,1,1),new Vector3(1,1,1), //front
            //    new Vector3(-1,-1,1),new Vector3(1,1,1),new Vector3(1,-1,1),


            //    new Vector3(1,-1,1),new Vector3(1,1,1),new Vector3(1,1,-1), //right
            //    new Vector3(1,-1,1),new Vector3(1,1,-1),new Vector3(1,-1,-1),


            //    new Vector3(1,-1,-1),new Vector3(1,1,-1),new Vector3(-1,1,-1), //back
            //    new Vector3(1,-1,-1),new Vector3(-1,1,-1),new Vector3(-1,-1,-1),


            //    new Vector3(-1,-1,-1),new Vector3(-1,1,-1),new Vector3(-1,1,1), //left
            //    new Vector3(-1,-1,-1),new Vector3(-1,1,1),new Vector3(-1,-1,1),


            //    new Vector3(-1,1,-1),new Vector3(-1,1,1),new Vector3(1,1,1), //top
            //    new Vector3(-1,1,-1),new Vector3(1,1,1),new Vector3(1,1,-1),


            //    new Vector3(-1,-1,1),new Vector3(-1,-1,-1),new Vector3(1,-1,-1), //bottom
            //    new Vector3(-1,-1,1),new Vector3(1,-1,-1),new Vector3(1,-1,1),



            //};

            //Vector3[] cubeNormal = new Vector3[]
            //{
            //    new Vector3(0,0,1),new Vector3(0,0,1),new Vector3(0,0,1), //front
            //    new Vector3(0,0,1),new Vector3(0,0,1),new Vector3(0,0,1),

            //    new Vector3(1,0,0),new Vector3(1,0,0),new Vector3(1,0,0), //right
            //    new Vector3(1,0,0),new Vector3(1,0,0),new Vector3(1,0,0),

            //    new Vector3(0,0,-1),new Vector3(0,0,-1),new Vector3(0,0,-1), //back
            //    new Vector3(0,0,-1),new Vector3(0,0,-1),new Vector3(0,0,-1),

            //    new Vector3(-1,0,0),new Vector3(-1,0,0),new Vector3(-1,0,0), //left
            //    new Vector3(-1,0,0),new Vector3(-1,0,0),new Vector3(-1,0,0),

            //    new Vector3(0,1,0),new Vector3(0,1,0),new Vector3(0,1,0), //top
            //    new Vector3(0,1,0),new Vector3(0,1,0),new Vector3(0,1,0),

            //    new Vector3(0,-1,0),new Vector3(0,-1,0),new Vector3(0,-1,0), //bottom
            //    new Vector3(0,-1,0),new Vector3(0,-1,0),new Vector3(0,-1,0),
            //};

            //Vector2[] cubeUV = new Vector2[]
            //{
            //    new Vector2(0,0),new Vector2(0,1),new Vector2(1,1),
            //    new Vector2(0,0),new Vector2(1,1),new Vector2(1,0),
                
            //    new Vector2(0,0),new Vector2(0,1),new Vector2(1,1),
            //    new Vector2(0,0),new Vector2(1,1),new Vector2(1,0),
                
            //    new Vector2(0,0),new Vector2(0,1),new Vector2(1,1),
            //    new Vector2(0,0),new Vector2(1,1),new Vector2(1,0),

            //    new Vector2(0,0),new Vector2(0,1),new Vector2(1,1),
            //    new Vector2(0,0),new Vector2(1,1),new Vector2(1,0),

            //    new Vector2(0,0),new Vector2(0,1),new Vector2(1,1),
            //    new Vector2(0,0),new Vector2(1,1),new Vector2(1,0),

            //    new Vector2(0,0),new Vector2(0,1),new Vector2(1,1),
            //    new Vector2(0,0),new Vector2(1,1),new Vector2(1,0),
            //};
            //uint[] cubeTris = new uint[cube.Length];
            //for(uint i=0;i<cubeTris.Length;i++)
            //{
            //    cubeTris[i] = i;
            //}

            //model = new GLModel(cube, cubeUV, cubeTris, cubeNormal, new Texture("tommycube.jpg"));

            model = new GLModel(new string[] { 
                "isabelle1.obj", "isabelle2.obj", "isabelle3.obj", "isabelle4.obj"
            }, new Texture[] {
                new Texture("b0.png"),new Texture("cloth.png"),new Texture("e.0.png"),new Texture("m.0.png")
            });

            watch = System.Diagnostics.Stopwatch.StartNew();

            Glut.glutMainLoop();
        }

        private static void OnDisplay()
        {

        }

        private static void OnClose()
        {
            model.Dispose();
            program.DisposeChildren = true;
            program.Dispose();
        }

        private static void OnSpecialInputDown(int key, int x, int y)
        {
            switch(key)
            {
                case Glut.GLUT_KEY_UP:
                    arrows[0] = true;
                    break;
                case Glut.GLUT_KEY_DOWN:
                    arrows[1] = true;
                    break;
                case Glut.GLUT_KEY_LEFT:
                    arrows[2] = true;
                    break;
                case Glut.GLUT_KEY_RIGHT:
                    arrows[3] = true;
                    break;
            }
        }

        private static void OnSpecialInputUp(int key, int x, int y)
        {
            switch (key)
            {
                case Glut.GLUT_KEY_UP:
                    arrows[0] = false;
                    break;
                case Glut.GLUT_KEY_DOWN:
                    arrows[1] = false;
                    break;
                case Glut.GLUT_KEY_LEFT:
                    arrows[2] = false;
                    break;
                case Glut.GLUT_KEY_RIGHT:
                    arrows[3] = false;
                    break;
            }
        }

        private static void OnKeyDown(byte key, int x, int y)
        {
            keys[key] = true;
        }

        private static void OnKeyUp(byte key, int x, int y)
        {
            keys[key] = false;
        }

        private static void OnRenderFrame()
        {
            watch.Stop();
            float deltaTime = (float)watch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency;
            watch.Restart();

            if (keys['w'])
            {
                rotation.X += deltaTime * rotFactor;
            }
            if (keys['s'])
            {
                rotation.X -= deltaTime * rotFactor;
            }

            if (keys['a'])
            {
                rotation.Y += deltaTime * rotFactor;
            }
            if (keys['d'])
            {
                rotation.Y -= deltaTime * rotFactor;
            }

            if (keys['q'])
            {
                rotation.Z += deltaTime * rotFactor;
            }
            if (keys['e'])
            {
                rotation.Z -= deltaTime * rotFactor;
            }

            if (arrows[0])
            {
                position.Y += deltaTime * movFactor;
            }
            if (arrows[1])
            {
                position.Y -= deltaTime * movFactor;
            }

            if (arrows[2])
            {
                position.X -= deltaTime * movFactor;
            }
            if (arrows[3])
            {
                position.X += deltaTime * movFactor;
            }

            if (keys['f'])
            {
                position.Z += deltaTime * movFactor;
            }
            if (keys['r'])
            {
                position.Z -= deltaTime * movFactor;
            }

            Gl.Viewport(0, 0, width, height);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Gl.UseProgram(program);
            foreach (GLMesh mesh in model.meshes)
            {
                Gl.BindTexture(mesh.tex);

                program["model_matrix"].SetValue(Matrix4.CreateRotationX(rotation.X) * Matrix4.CreateRotationY(rotation.Y) * Matrix4.CreateRotationZ(rotation.Z) * Matrix4.CreateTranslation(position));
                Gl.BindBufferToShaderAttribute(mesh.verts, program, "vertexPosition");
                Gl.BindBufferToShaderAttribute(mesh.normals, program, "vertexNormal");
                Gl.BindBufferToShaderAttribute(mesh.uvCoords, program, "vertexUV");
                Gl.BindBuffer(mesh.tris);

                Gl.DrawElements(BeginMode.Triangles, mesh.tris.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            }

            Glut.glutSwapBuffers();
        }

        public static string VertexShader = @"
#version 130

in vec3 vertexPosition;
in vec3 vertexNormal;
in vec2 vertexUV;

out vec3 normal;
out vec2 uv;
            
uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;

void main(void)
{
    normal = normalize((model_matrix * vec4(vertexNormal, 0)).xyz);
    uv = vertexUV;
    gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
}
";

        public static string FragmentShader = @"
#version 130

uniform vec3 light_direction;
uniform sampler2D texture;

in vec2 uv;
in vec3 normal;

out vec4 fragment;

void main(void)
{
    float diffuse = max(dot(normal, light_direction) ,0);
    float ambient = 0.3;
    float lighting = max(diffuse, ambient);
    vec4 sample = texture2D(texture, uv);
    fragment = vec4(sample.xyz * lighting, sample.a);
}
";
    }
}
