using Autofac.Core;

namespace Autofac.samples
{
    public interface ILog
    {
        void Write(string message);
    }
    public interface IConsole
    {
    }
    public class ConsoleLog :ILog
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class EmailLog : ILog,IConsole
    {
        private const string adminEmail = "admin@foo.com";
        public void Write(string message)
        {
            Console.WriteLine($"Email sent to {adminEmail} : {message}");
        }
    }
    public class Engine
    {
        private ILog log;
        private int id;

        public Engine(ILog log)
        {
            this.log = log;
            id = new Random().Next();
        }

        public Engine(ILog log, int id)
        {
            this.log = log;
            this.id = id;
        }

        public void Ahead(int power)
        {
            log.Write($"Engine[{id}] ahead {power}");
        }
    }

    public class SMSLog : ILog
    {
        private string phoneNumber;
        public SMSLog(string phoneNumber)
        {
            this.phoneNumber = phoneNumber;
        }
        public void Write(string message)
        {
            Console.WriteLine($"SMS to {phoneNumber} : {message}");
        }
    }
    public class Car
    {
        private Engine engine;
        private ILog log;

        public Car(Engine engine)
        {
            this.engine = engine;
            this.log = new EmailLog();
        }
        public Car(Engine engine, ILog log)
        {
            this.engine = engine;
            this.log = log;
        }

        public void Go()
        {
            engine.Ahead(100);
            log.Write($"Car going forward...");
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            // named parameter
            // builder.RegisterType<SMSLog>()
            //     .As<ILog>()
            //     .WithParameter("phoneNumber", "+123456789");
            
            //type parameter
            // builder.RegisterType<SMSLog>()
            //     .As<ILog>()
            //     .WithParameter(new TypedParameter(typeof(string), "_123456789"));
            
            
            // builder.RegisterType<SMSLog>()
            //     .As<ILog>()
            //     .WithParameter(
            //         // predicate
            //         new ResolvedParameter(
            //     (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "phoneNumber",
            //             // value acessor
            // (pi,ctx) => "+123456678"
            //         )
            //      );
            //
            // var container = builder.Build();
            // var log = container.Resolve<ILog>();
            // log.Write($"test message");


            Random random = new Random();

            builder.Register
            ((c, p)
                => new SMSLog(p.Named<string>($"phoneNumber")))
                .As<ILog>();
            
            Console.WriteLine($"about to build container...");
            var container = builder.Build();

            var log = container.Resolve<ILog>
                (new NamedParameter($"phoneNumber", random.Next().ToString()));
            log.Write($"testing");
        }
    }
}
