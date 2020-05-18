using System.Collections.Generic;

namespace FWSCRM.Models
{
    public class Menu
    {
        public Menu()
        {
            MenuItems = new List<Menu>();
        }

        public int Id { get; set; }
        public int IdPai { get; set; }
        public string IdMenu { get; set; }
        public string NomeMenu { get; set; }
        public string Url { get; set; }
        public List<Menu> MenuItems { get; set; }
    }
}