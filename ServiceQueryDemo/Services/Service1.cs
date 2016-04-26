using System.Collections.Generic;
using TITcs.SharePoint.Services;

namespace ServiceQueryDemo.Services
{
    public class Service1 : ServiceBase
    {
        [Route]
        public object GetAll()
        {
            return Ok(new
            {
                titulo = "Titulo 1",
                descricao = "Descricao 1",
                usuarios = new List<string>()
                {
                    "stiven.camara",
                    "marcos.natan"
                }

            });
        }
    }
}