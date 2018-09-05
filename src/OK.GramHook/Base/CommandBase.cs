using System;
using System.Threading.Tasks;

namespace OK.GramHook
{
    public abstract class CommandBase
    {
        public CommandContext Context { get; set; }

        internal bool IsAborted { get; set; }

        internal Func<string, Task> OnReplyAsync { get; set; }

        public virtual Task OnPreExecutionAsync()
        {
            return Task.CompletedTask;
        }

        protected Task ReplyAsync(string text)
        {
            return OnReplyAsync?.Invoke(text);
        }

        protected virtual Task AbortAsync()
        {
            IsAborted = true;

            return Task.CompletedTask;
        }

        public virtual Task OnPostExecutionAsync()
        {
            return Task.CompletedTask;
        }
    }
}