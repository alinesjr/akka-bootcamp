using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Akka.Actor;

namespace WinTail
{
    public class FileValidatorActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;

        public FileValidatorActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as string;
            if (String.IsNullOrEmpty(msg))
            {
                _consoleWriterActor.Tell(new Messages.NullInputError("Input was blank. Please try again.\n"));
                Sender.Tell(new Messages.ContinueProcessing());
            }
            else
            {
                if (IsFileUri(msg))
                {
                    _consoleWriterActor.Tell(new Messages.InputSuccess(String.Format("Starting processing for {0}", msg)));

                    Context.ActorSelection("akka://MyActorSystem/user/tailCoordinatorActor").Tell(new TailCoordinatorActor.StartTail(msg, _consoleWriterActor));
                }
                else
                {
                    _consoleWriterActor.Tell(new Messages.ValidationError(String.Format("{0} is not an existing URI on disk.", msg)));
                    Sender.Tell(new Messages.ContinueProcessing());
                }
            }
        }

        private bool IsFileUri(string path)
        {
            return File.Exists(path);
        }
    }
}
