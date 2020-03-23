using Halogen.Core.Services;

namespace Halogen.Core
{
    public class HalogenClient
    {
        private MetadataEmbedService _embedService;

        public HalogenClient(MetadataEmbedService embedService)
        {
            _embedService = embedService;
        }
        
        
    }
}