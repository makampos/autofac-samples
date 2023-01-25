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
    public class Service
    {
        public string DoSomething(int value)
        {
            return $"I have {value}";
        }
    }

    public class DomainObject
    {
        private Service service;
        private int value;

        public delegate DomainObject Factory(int value);
        
         public DomainObject(Service service, int value)
         {
             this.service = service ?? throw new ArgumentNullException(nameof(service));
             this.value = value;
         }
         public override string ToString()
         {
             return service.DoSomething(value);
         }
    }
    
    public class Entity
    {
        public delegate Entity Factory();
        private static Random random = new Random();
        private int number;

        public Entity()
        {
            number = random.Next();
        }

        public override string ToString()
        {
            return $" test " + number;
        }
    }

    public class ViewModel
    {
        private readonly Entity.Factory entityFactory;

        public ViewModel(Entity.Factory entityFactory)
        {
            this.entityFactory = entityFactory;
        }

        public void Method()
        {
            var entity = entityFactory();
            Console.WriteLine(entity);
        }
    }

    public class Parent
    {
        public override string ToString()
        {
            return $"I'm your father";
        }
    }

    public class Child
    {
        public string Name { get; set; }
        public Parent Parent { get; set; }
        public void SetParent(Parent parent)
        {
            Parent = parent;
        }
    }
    internal class Program
    {
        public static void Main(string[] args)
        {
            // var builder = new ContainerBuilder();
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
            

            // var builder = new ContainerBuilder();
            // Random random = new Random();
            // builder.Register
            // ((c, p)
            //     => new SMSLog(p.Named<string>($"phoneNumber")))
            //     .As<ILog>();
            // Console.WriteLine($"about to build container...");
            // var container = builder.Build();
            // var log = container.Resolve<ILog>
            //     (new NamedParameter($"phoneNumber", random.Next().ToString()));
            // log.Write($"testing");

            
            // // Delegate factory
            // var cb = new ContainerBuilder();
            // cb.RegisterType<Service>();
            // cb.RegisterType<DomainObject>();
            // var container = cb.Build();
            // var dobj = container.Resolve<DomainObject>(
            //     new PositionalParameter(1, 42));
            // Console.WriteLine(dobj);
            // var factory = container.Resolve<DomainObject.Factory>();
            // var dobj2 = factory(42);
            // Console.WriteLine(dobj2);

            
            //  // Object on demand
            // var cb = new ContainerBuilder();
            // cb.RegisterType<Entity>().InstancePerDependency();
            // cb.RegisterType<ViewModel>();
            // var container = cb.Build();
            // var vm = container.Resolve<ViewModel>();
            // vm.Method();
            // vm.Method();

            var builder = new ContainerBuilder();
            builder.RegisterType<Parent>();
            // builder.RegisterType<Child>().PropertiesAutowired();
            // builder.RegisterType<Child>()
            //     .WithProperty($"Parent", new Parent());

            // builder.Register(c =>
            // {
            //     var child = new Child();
            //     child.SetParent(c.Resolve<Parent>());
            //     return child;
            // });
            
            builder.RegisterType<Child>()
                .OnActivated(e =>
                {
                    var parent = e.Context.Resolve<Parent>();
                    e.Instance.SetParent(parent);
                });
            
            var container = builder.Build();
            var parent = container.Resolve<Child>().Parent;
            Console.WriteLine(parent);
        }
    }
}
