namespace Entities.Models
{
    // Because we implement HATEOAS into our API
    // so when using DataShaped that allow client to choose which data that server
    // need to return => it will hide the Id column which HATEOAS need for generate link
    // so we need to create ShapedEntity that contain an Entity and an Id to generate HATEOAS link
    public class ShapedEntity
    {
        public ShapedEntity()
        {
            Entity = new Entity();
        }
        public Guid Id { get; set; }

        public Entity Entity { get; set; }
    }

}
