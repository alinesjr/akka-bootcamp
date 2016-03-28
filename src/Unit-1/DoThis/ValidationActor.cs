using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Akka.Actor;

namespace WinTail
{
    public class ValidationActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;

        public ValidationActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as string;
            if (String.IsNullOrEmpty(msg))
            {
                _consoleWriterActor.Tell(new Messages.InputError("No input received"));
            }
            else
            {
                if (IsValid(msg))
                {
                    _consoleWriterActor.Tell(new Messages.InputSuccess("Thank you! Message was valid."));
                }
                else
                {
                    _consoleWriterActor.Tell(new Messages.ValidationError("Invalid: input had an odd # of characters."));
                }
            }

            Sender.Tell(new Messages.ContinueProcessing());
        }

        private bool IsValid(string msg)
        {
            return msg.Length % 2 == 0;
        }
    }
}
