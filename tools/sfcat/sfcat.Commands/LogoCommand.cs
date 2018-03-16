using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat
{
    internal class LogoCommand: Command
    {
        protected override Intention RunCommand()
        {
            return new MultiOutputConclusion(new string[]
                {
                    "",
                    @"#w /\     /\",
                    @"#w{  `---'  }",
                    @"#w{  #gO#~   #yO#~  }",
                    @"#w~~>  V  <~~            #cService Fabric Catalog Service",
                    @"#w \  \|/  /             ==============================",
                    @"#w  `-----'____          Version 0.1",
                    @"#w  /     \    \_        December 2017",
                    @"#w {       }\  )_\_   _  Azure CTO's Office",
                    @"#w |  \_/  |/ /  \_\_/ ) ==============================",
                    @"#w  \__/  /(_/     \__/",
                    @"#w    (__/",
                    ""});
        }
    }
}
