using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class PixelationRenderFeature : ScriptableRendererFeature
    {
        PixelationPass pixelationPass;

        public override void Create()
        {
            this.pixelationPass = new PixelationPass(RenderPassEvent.BeforeRenderingPostProcessing);
        }

        //ScripstableRendererFeature is an abstract class, you need this method
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            this.pixelationPass.Setup(renderer.cameraColorTargetHandle);
            renderer.EnqueuePass(this.pixelationPass);
        }
    }
    
    
    public class PixelationPass : ScriptableRenderPass
    {
        private static readonly string shaderPath = "PostEffect/Pixelation";
        static readonly string k_RenderTag = "Render Pixelation Effects";
        static readonly int MainTexId = Shader.PropertyToID("_MainTex");
        static readonly int TempTargetId = Shader.PropertyToID("_TempTargetPixelation");
        
        //PROPERTIES
        static readonly int WidthPixelation = Shader.PropertyToID("_WidthPixelation");
        static readonly int HeightPixelation = Shader.PropertyToID("_HeightPixelation");
        static readonly int ColorPrecison = Shader.PropertyToID("_ColorPrecision");

        
        Pixelation pixelation;
        Material pixelationMaterial;
        RenderTargetIdentifier currentTarget;
    
        public PixelationPass(RenderPassEvent evt)
        {
            this.renderPassEvent = evt;
            Shader shader = Shader.Find(shaderPath);
            if (shader == null)
            {
                Debug.LogError("Shader not found.");
                return;
            }
            this.pixelationMaterial = CoreUtils.CreateEngineMaterial(shader);
        }
    
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (this.pixelationMaterial == null)
            {
                Debug.LogError("Material not created.");
                return;
            }
    
            if (!renderingData.cameraData.postProcessEnabled) return;
    
            VolumeStack stack = VolumeManager.instance.stack;
            
            this.pixelation = stack.GetComponent<Pixelation>();
            if (this.pixelation == null) { return; }
            if (!this.pixelation.IsActive()) { return; }
    
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
            this.pixelationMaterial.SetFloat(WidthPixelation, this.pixelation.widthPixelation.value);
            this.pixelationMaterial.SetFloat(HeightPixelation, this.pixelation.heightPixelation.value);
            this.pixelationMaterial.SetFloat(ColorPrecison, this.pixelation.colorPrecision.value);

            int shaderPass = 0;
            cmd.SetGlobalTexture(MainTexId, source);
            cmd.GetTemporaryRT(destination, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
            cmd.Blit(source, destination);
            cmd.Blit(destination, source, this.pixelationMaterial, shaderPass);
        }
    }
}