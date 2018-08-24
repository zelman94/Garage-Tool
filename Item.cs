using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GarageTool
{
    public class Item
    {
        private string name;
        public string Name { get { return name; } }
        private string id;
        public string Id { get { return id; } }
        private string validation;
        public string Validation { get { return validation; } }

        private Point position;
        public Point Position { get { return position; } }
        public Item(string name,string id, Point position)
        {
            this.name = name;
            this.id = id;
            this.position = position;
        }

        public Item(string name, string id, Point position, string owner, string lokal, string status,string validation="")
        {
            this.name = name;
            this.id = id;
            this.position = position;
            this.owner = owner;
            this.lokal = lokal;
            this.status = status;
            this.validation = validation;
        }


        private string owner;
        public string Owner { get { return owner; } }

        private string lokal;
        public string Lokal { get { return lokal; } }

        private string status;
        public string Status { get { return status; } }
        public string Get_stringfor_QR(Item tmp)
        {
            return $"{tmp.name};{tmp.id};{tmp.Position.X};{tmp.Position.Y};{tmp.lokal};{tmp.Owner};{tmp.status}";
        }
    }
}
