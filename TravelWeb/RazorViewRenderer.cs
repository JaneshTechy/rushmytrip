using Microsoft.AspNetCore.Hosting;
using RazorLight;
using System.Threading.Tasks;

namespace TravelWeb
{
    public class RazorViewRenderer
    {
        private readonly RazorLightEngine _engine;

        public RazorViewRenderer(IWebHostEnvironment env)
        {
            _engine = new RazorLightEngineBuilder().UseFileSystemProject(env.ContentRootPath).Build();
        }

        public async Task<string> RenderAsync<T>(string viewPath, T model)
        {
            return await _engine.CompileRenderAsync(viewPath, model);
        }
    }
}
