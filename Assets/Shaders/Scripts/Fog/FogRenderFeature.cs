using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class FogRenderFeature : ScriptableRendererFeature
    {
        FogPass fogPass;

        public override void Create()
        {
            this.fogPass = new FogPass(RenderPassEvent.BeforeRenderingPostProcessing);
        }

        //ScripstableRendererFeature is an abstract class, you need this method
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            this.fogPass.Setup(renderer.cameraColorTargetHandle);
            renderer.EnqueuePass(this.fogPass);
        }
    }
    
    
    public class FogPass : ScriptableRenderPass
    {
        private static readonly string shaderPath = "PostEffect/Fog";
        static readonly string k_RenderTag = "Render Fog Effects";
        static readonly int MainTexId = Shader.PropertyToID("_MainTex");
        static readonly int TempTargetId = Shader.PropertyToID("_TempTargetFog");
        static readonly int FogDensity = Shader.PropertyToID("_FogDensity");
        static readonly int FogDistance = Shader.PropertyToID("_FogDistance");
        static readonly int FogColor = Shader.PropertyToID("_FogColor");
        static readonly int FogNear = Shader.PropertyToID("_FogNear");
        static readonly int FogFar = Shader.PropertyToID("_FogFar");
        static readonly int FogAltScale = Shader.PropertyToID("_FogAltScale");
        static readonly int FogThinning = Shader.PropertyToID("_FogThinning");
        static readonly int NoiseScale = Shader.PropertyToID("_NoiseScale");
        static readonly int NoiseStrength = Shader.PropertyToID("_NoiseStrength");
        
        Fog fog;
        Material fogMaterial;
        RenderTargetIdentifier currentTarget;
    
        public FogPass(RenderPassEvent evt)
        {
            this.renderPassEvent = evt;
            Shader shader = Shader.Find(shaderPath);
            if (shader == null)
            {
                Debug.LogError("Shader not found.");
                return;
            }
            this.fogMaterial = CoreUtils.CreateEngineMaterial(shader);
        }
    
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (this.fogMaterial == null)
            {
                Debug.LogError("Material not created.");
                return;
            }
    
            if (!renderingData.cameraData.postProcessEnabled) return;
    
            VolumeStack stack = VolumeManager.instance.stack;
            
            this.fog = stack.GetComponent<Fog>();
            if (this.fog == null) { return; }
            if (!this.fog.IsActive()) { return; }
    
            CommandBuffer cmd = CommandBufferPool.Get(k_RenderTag);
            Render(cmd, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    
        public void Setup(in RenderTargetIdentifier currentTarget)
        {
            this.currentTarget = currentTarget;
        }
    
        void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ref CameraData cameraData = ref renderingData.cameraData;
            RenderTargetIdentifier source = this.currentTarget;
            int destination = TempTargetId;
    
            //getting camera width and height 
            int w = cameraData.camera.scaledPixelWidth;
            int h = cameraData.camera.scaledPixelHeight;
            
            //setting parameters here 
            cameraData.camera.depthTextureMode = cameraData.camera.depthTextureMode | DepthTextureMode.Depth;
            this.fogMaterial.SetFloat(FogDensity, this.fog.fogDensity.value);
            this.fogMaterial.SetFloat(FogDistance, this.fog.fogDistance.value);
            this.fogMaterial.SetColor(FogColor, this.fog.fogColor.value);
            this.fogMaterial.SetFloat(FogNear, this.fog.fogNear.value);
            this.fogMaterial.SetFloat(FogFar, this.fog.fogFar.value);
            this.fogMaterial.SetFloat(FogAltScale, this.fog.fogAltScale.value);
            this.fogMaterial.SetFloat(FogThinning, this.fog.fogThinning.value);
            this.fogMaterial.SetFloat(NoiseScale, this.fog.noiseScale.value);
            this.fogMaterial.SetFloat(NoiseStrength, this.fog.noiseStrength.value);
    
            int shaderPass = 0;
            cmd.SetGlobalTexture(MainTexId, source);
            cmd.GetTemporaryRT(destination, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
            cmd.Blit(source, destination);
            cmd.Blit(destination, source, this.fogMaterial, shaderPass);
        }
    }
}