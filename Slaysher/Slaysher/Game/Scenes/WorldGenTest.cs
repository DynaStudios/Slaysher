using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Slaysher.Game.GUI.Components;
using Slaysher.Game.GUI.Screens;
using Slaysher.Game.Scenes.WorldGen;
using Slaysher.Graphics.Camera;
using SlaysherNetworking.Game.World;

namespace Slaysher.Game.Scenes
{
    public class WorldGenTest : GameScreen
    {
        private Random _random;
        private Model _pattern;
        private Texture2D _perlinNoiseTexture;

        private Matrix _worldMatrix;
        private readonly Camera _tempCamera = new Camera(new SlaysherNetworking.Game.World.WorldPosition());
        public Camera Camera { get { return _tempCamera; } }

        private bool Generating { get; set; }

        #region Overrides of GameScreen

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);
            WorldGenMenu menu = new WorldGenMenu() {ZIndex = 1};
            
            Button generateButton = new Button("Generate");
            generateButton.Clicked += GenerateButtonClicked;

            menu.PanelEntries.Add(generateButton);

            ScreenManager.AddScreen(menu);

            //Load other content to generate stuff
            _random = new Random(0);

            _worldMatrix = Matrix.Identity;
            Camera.Target = new WorldPosition(0, 0);
            _pattern = ScreenManager.Game.Content.Load<Model>("Models/Pattern/Pattern");

            //Render first perlin noise before hiding loading screen...
            GeneratePerlinNoise();
        }

        private void GenerateButtonClicked(object sender, EventArgs e)
        {
            Generating = true;
            _random = new Random();

            GeneratePerlinNoise();

            //Create new Perlin noise texture here
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, true, false);

            _tempCamera.Update(_worldMatrix);
        }

        public override void Draw(GameTime gameTime)
        {
            if(!Generating)
                DrawPattern(_pattern, _worldMatrix, Camera);

            base.Draw(gameTime);
        }

        public void DrawPattern(Model model, Matrix worldMatrix, Camera camera)
        {
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.EnableDefaultLighting();
                    //effect.World = modelTransforms[mesh.ParentBone.Index] * worldMatrix * _position;
                    effect.TextureEnabled = true;
                    effect.Texture = _perlinNoiseTexture;

                    effect.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                    //effect.CurrentTechnique.Passes[0].Apply();

                    effect.World = modelTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(new Vector3(0f, 0f, 0f));
                    effect.View = camera.ViewMatrix;
                    effect.Projection = camera.ProjectionMatrix;
                }
                mesh.Draw();
            }
        }

        #endregion

        private void GeneratePerlinNoise()
        {
            int width = 512;
            int height = 512;
            int octaveCount = 7;

            Color gradientStart = Color.Green;
            Color gradiendEnd = Color.Brown;

            Texture2D grass = ScreenManager.Game.Content.Load<Texture2D>("Images/Game/Textures/grass_512");
            Texture2D dirt = ScreenManager.Game.Content.Load<Texture2D>("Images/Game/Textures/dirt_512_j");

            float[][] perlinNoise = GeneratePerlinNoise(width, height, octaveCount);
            _perlinNoiseTexture = MapGradient(gradientStart, gradiendEnd, perlinNoise);
            //_perlinNoiseTexture = TextureBlendedNoise(dirt, grass, perlinNoise);
            Generating = false;
        }

        public float[][] GenerateWhiteNoise(int width, int height)
        {
            float[][] noise = GetEmptyArray<float>(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    noise[i][j] = (float)_random.NextDouble() % 1;
                }
            }

            return noise;
        }

        public float [][] GenerateSmoothNoise(float[][] baseNoise, int octave)
        {
            int width = baseNoise.Length;
            int height = baseNoise[0].Length;

            float[][] smoothNoise = GetEmptyArray<float>(width, height);

            int samplePeriod = 1 << octave; // calculates 2 ^ k
            float sampleFrequency = 1.0f / samplePeriod;

            for (int i = 0; i < width; i++)
            {
                //calculate the horizontal sampling indices
                int sample_i0 = (i / samplePeriod) * samplePeriod;
                int sample_i1 = (sample_i0 + samplePeriod) % width; //wrap around
                float horizontal_blend = (i - sample_i0) * sampleFrequency;

                for (int j = 0; j < height; j++)
                {
                    //calculate the vertical sampling indices
                    int sample_j0 = (j / samplePeriod) * samplePeriod;
                    int sample_j1 = (sample_j0 + samplePeriod) % height; //wrap around
                    float vertical_blend = (j - sample_j0) * sampleFrequency;

                    //blend the top two corners
                    float top = Interpolate(baseNoise[sample_i0][sample_j0],
                        baseNoise[sample_i1][sample_j0], horizontal_blend);

                    //blend the bottom two corners
                    float bottom = Interpolate(baseNoise[sample_i0][sample_j1],
                        baseNoise[sample_i1][sample_j1], horizontal_blend);

                    //final blend
                    smoothNoise[i][j] = Interpolate(top, bottom, vertical_blend);
                }
            }

            return smoothNoise;
        }

        public float[][] GeneratePerlinNoise(float[][] baseNoise, int octaveCount)
        {
            int width = baseNoise.Length;
            int height = baseNoise[0].Length;

            float[][][] smoothNoise = new float[octaveCount][][]; //an array of 2D arrays containing

            float persistance = 0.7f;

            //generate smooth noise
            for (int i = 0; i < octaveCount; i++)
            {
                smoothNoise[i] = GenerateSmoothNoise(baseNoise, i);
            }

            float[][] perlinNoise = GetEmptyArray<float>(width, height); //an array of floats initialised to 0

            float amplitude = 1.0f;
            float totalAmplitude = 0.0f;

            //blend noise together
            for (int octave = octaveCount - 1; octave >= 0; octave--)
            {
                amplitude *= persistance;
                totalAmplitude += amplitude;

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        perlinNoise[i][j] += smoothNoise[octave][i][j] * amplitude;
                    }
                }
            }

            //normalisation
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    perlinNoise[i][j] /= totalAmplitude;
                }
            }

            return perlinNoise;
        }

        public float[][] GeneratePerlinNoise(int width, int height, int octaveCount)
        {
            float[][] baseNoise = GenerateWhiteNoise(width, height);
            return GeneratePerlinNoise(baseNoise, octaveCount);
        }

        private Texture2D TextureBlendedNoise(Texture2D grass, Texture2D dirt, float[][] perlinNoise)
        {
            int width = perlinNoise.Length;
            int height = perlinNoise[0].Length;

            Color[] grassTex = new Color[grass.Width * grass.Height];
            Color[] dirtTex = new Color[dirt.Width * dirt.Height];

            grass.GetData(grassTex);
            dirt.GetData(dirtTex);

            Texture2D permTexture2d = new Texture2D(ScreenManager.GraphicsDevice, width, height, true,
                                                                                     SurfaceFormat.Color);

            Color[] color = TextureBlended(grassTex, dirtTex, perlinNoise);
            permTexture2d.SetData(color);

            return permTexture2d;
        }

        public Color[] TextureBlended(Color[] image1, Color[] image2, float[][] perlinNoise)
        {
            int width = 512;
            int heigth = 512;

            Color[] image = new Color[width * heigth];

            for (int i = 0; i < width; i++)
            {
                for(int j = 0; j < heigth; j++)
                {
                    image[i + (j*width)] = Interpolate(image1[i + (j*width)], image2[i + (j*width)], perlinNoise[i][j]);
                }
            }

            return image;
        }

        public Texture2D MapGradient(Color gradientStart, Color gradientEnd, float[][] perlinNoise)
        {
            int width = perlinNoise.Length;
            int height = perlinNoise[0].Length;

            Texture2D permTexture2d = new Texture2D(ScreenManager.GraphicsDevice, width, height, true,
                                                                                     SurfaceFormat.Color);
            Color[] data = new Color[width * height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    data[i + (j*width)] = GetColor(gradientStart, gradientEnd, perlinNoise[i][j]);
                }
            }

            permTexture2d.SetData<Color>(data);
            return permTexture2d;
        }

        public static Color GetAlternativeColor(Color gradientStart, Color gradientEnd, float t)
        {
            float u = 1 - t;

            Color color = new Color((int)(255 * u + 255 * t), 0, 0);

            return color;
        }

        public static Color GetColor(Color gradientStart, Color gradientEnd, float t)
        {
            float u = 1 - t;

            Color color = new Color((int) (gradientStart.R * u + gradientEnd.R * t),
                (int)(gradientStart.G * u + gradientEnd.G * t),
                (int)(gradientStart.B * u + gradientEnd.B * t));

            return color;
        }

        public static float Interpolate(float x0, float x1, float alpha)
        {
            return x0*(1 - alpha) + alpha*x1;
        }

        public static Color Interpolate(Color col0, Color col1, float alpha)
        {
            float beta = 1 - alpha;
            return new Color((int)(col0.R * alpha + col1.R * beta), (int)(col0.G * alpha + col1.G * beta), (int)(col0.B * alpha + col1.B * beta));
        }

        public static T[][] GetEmptyArray<T>(int width, int height)
        {
            T[][] arr = new T[width][];

            for (int i = 0; i < width; i++)
            {
                arr[i] = new T[height];
            }

            return arr;
        }

    }
}