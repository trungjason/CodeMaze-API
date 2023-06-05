using Entities.Models;

namespace Entities.LinkModels
{
    // With this class
    // we can check HasLinks property to decide
    // which Entities we should return to client.
    // If we use HATOES => return LinkedEntities
    // otherwise we just return ShapedEntitites
    public class LinkResponse
    {
        public bool HasLinks { get; set; }

        public List<Entity> ShapedEntities { get; set; }

        public LinkCollectionWrapper<Entity> LinkedEntities { get; set; }

        public LinkResponse()
        {
            LinkedEntities = new LinkCollectionWrapper<Entity>();
            ShapedEntities = new List<Entity>();
        }
    }
}
