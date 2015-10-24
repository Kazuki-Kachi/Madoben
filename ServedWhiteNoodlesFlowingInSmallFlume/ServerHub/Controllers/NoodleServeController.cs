using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ServedWhiteNoodlesFlowingInSmallFlumeLibraries;
using ServerHub.Hubs;

namespace ServerHub.Controllers
{
    public class NoodleServeController : HubController<PushHub>
    {   
        public async Task<IHttpActionResult> PostAAA(IReadOnlyList<INoodle> noodles)
        {
            
            return Ok();
        }
    }
}
