using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GarageTool
{
    public class Device
    {
        private string name;
        public string Name { get { return name; } }
        private string id;
        public string Id { get { return id; } }
       

        private Point position;
        public Point Position { get { return position; } }
        public Device(string name,string id, Point position)
        {
            this.name = name;
            this.id = id;
            this.position = position;
        }
        public Device() { }

        public Device(string name, string id, Point position, string owner, string lokal, string status)
        {
            this.name = name;
            this.id = id;
            this.position = position;
            this.owner = owner;
            this.lokal = lokal;
            this.status = status;
        }


        private string owner;
        public string Owner { get { return owner; } }

        private string lokal;
        public string Lokal { get { return lokal; } }

        private string status;
        public string Status { get { return status; } }
        public string Get_stringfor_QR(Device tmp)
        {
            return $"{tmp.name};{tmp.id};{tmp.Position.X};{tmp.Position.Y};{tmp.lokal};{tmp.Owner};{tmp.status}";
        }
    }
}
