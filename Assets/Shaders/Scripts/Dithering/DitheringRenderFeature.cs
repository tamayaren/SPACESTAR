using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class DitheringRenderFeature : ScriptableRendererFeature
    {
        DitheringPass ditheringPass;

        public override void Create()
        {
            this.ditheringPass = new DitheringPass(RenderPassEvent.BeforeRenderingPostProcessing);
        }

        //ScripstableRendererFeature is an abstract class, you need this method
        [Obsolete("Obsolete")]
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            this.ditheringPass.Setup(renderer.cameraColorTargetHandle);
            renderer.EnqueuePass(this.ditheringPass);
        }
    }
    
    
    public class DitheringPass : ScriptableRenderPass
    {
        private static readonly string shaderPath = "PostEffect/Dithering";
        static readonly string k_RenderTag = "Render Dithering Effects";
        static readonly int MainTexId = Shader.PropertyToID("_MainTex");
        static readonly int TempTargetId = Shader.PropertyToID("_TempTargetDithering");
        
        //PROPERTIES
        static readonly int PatternIndex = Shader.PropertyToID("_PatternIndex");
        static readonly int DitherThreshold = Shader.PropertyToID("_DitherThreshold");
        static readonly int DitherStrength = Shader.PropertyToID("_DitherStrength");
        static readonly int DitherScale = Shader.PropertyToID("_DitherScale");
        
        Dithering dithering;
        Material ditheringMaterial;
        RenderTargetIdentifier currentTarget;
    
        public DitheringPass(RenderPassEvent evt)
        {
            this.renderPassEvent = evt;
            Shader shader = Shader.Find(shaderPath);
            if (shader == null)
            {
                Debug.LogError("Shader not found.");
                return;
            }
            this.ditheringMaterial = CoreUtils.CreateEngineMaterial(shader);
        }
    
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (this.ditheringMaterial == null)
            {
                Debug.LogError("Material not created.");
                return;
            }
    
            if (!renderingData.cameraData.postProcessEnabled) return;
    
            VolumeStack stack = VolumeManager.instance.stack;
            
            this.dithering = stack.GetComponent<Dithering>();
            if (this.dithering == null) { return; }
            if (!this.dithering.IsActive()) { return; }
    
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
            this.ditheringMaterial.SetInt(PatternIndex, this.dithering.patternIndex.value);
            this.ditheringMaterial.SetFloat(DitherThreshold, this.dithering.ditherThreshold.value);
            this.ditheringMaterial.SetFloat(DitherStrength, this.dithering.ditherStrength.value);
            this.ditheringMaterial.SetFloat(DitherScale, this.dithering.ditherScale.value);

            int shaderPass = 0;
            cmd.SetGlobalTexture(MainTexId, source);
            cmd.GetTemporaryRT(destination, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
            cmd.Blit(source, destination);
            cmd.Blit(destination, source, this.ditheringMaterial, shaderPass);
        }
    }
}