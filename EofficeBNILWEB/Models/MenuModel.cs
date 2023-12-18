namespace EofficeBNILWEB.Models
{
    public class MenuOutput
    {
        public Guid idMenu { get; set; }
        public string menu { get; set; }
        public string authorizations { get; set; }
        public string link { get; set; }
        public Guid? parentId { get; set; }
        public int menuOrder { get; set; }
        public char hasChild { get; set; }
        public string? icon { get; set; }
        public List<MenuOutput> childMenu { get; set; }

        public string menu_en { get; set; }
    }
}
