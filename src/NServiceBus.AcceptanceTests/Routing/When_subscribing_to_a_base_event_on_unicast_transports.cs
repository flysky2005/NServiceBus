namespace NServiceBus.AcceptanceTests.Routing
{
    using System.Threading.Tasks;
    using AcceptanceTesting;
    using EndpointTemplates;
    using Features;
    using NUnit.Framework;

    public class When_subscribing_to_a_base_event_on_unicast_transports : NServiceBusAcceptanceTest
    {
        [Test]
        public async Task Specific_event_should_be_delivered()
        {
            var context = await Scenario.Define<Context>()
                .WithEndpoint<Publisher>(b => b.When(c => c.SubscriberSubscribed, async session => await session.Publish(new SpecificEvent())))
                .WithEndpoint<GeneralSubscriber>(b => b.When(async (session, c) => await session.Subscribe<IBaseEvent>()))
                .Done(c => c.SubscriberGotEvent)
                .Run();

            Assert.True(context.SubscriberGotEvent);
        }

        [Test]
        public async Task Base_event_should_only_be_delivered()
        {
            var context = await Scenario.Define<Context>()
                .WithEndpoint<Publisher>(b => b.When(c => c.SubscriberSubscribed, async session => await session.Publish<IBaseEvent>()))
                .WithEndpoint<GeneralSubscriber>(b => b.When(async (session, c) => await session.Subscribe<IBaseEvent>()))
                .Done(c => c.SubscriberGotEvent)
                .Run();

            Assert.True(context.SubscriberGotEvent);
        }

        public class Context : ScenarioContext
        {
            public bool SubscriberGotEvent { get; set; }

            public bool SubscriberSubscribed { get; set; }
        }

        public class Publisher : EndpointConfigurationBuilder
        {
            public Publisher()
            {
                EndpointSetup<DefaultPublisher>(b => b.OnEndpointSubscribed<Context>((args, context) =>
                {
                    context.SubscriberSubscribed = true;
                }));
            }
        }

        public class GeneralSubscriber : EndpointConfigurationBuilder
        {
            public GeneralSubscriber()
            {
                EndpointSetup<DefaultServer>(c =>
                {
                    c.DisableFeature<AutoSubscribe>();
                })
                    .AddMapping<IBaseEvent>(typeof(Publisher));
            }

            public class MyEventHandler : IHandleMessages<IBaseEvent>
            {
                public Context Context { get; set; }

                public Task Handle(IBaseEvent messageThatIsEnlisted, IMessageHandlerContext context)
                {
                    Context.SubscriberGotEvent = true;
                    return Task.FromResult(0);
                }
            }
        }

        public class SpecificEvent : IBaseEvent
        {
        }

        public interface IBaseEvent : IEvent
        {
        }
    }
}