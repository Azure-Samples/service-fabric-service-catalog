using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat
{
    public class Intention
    {
        public event EventHandler<ConclusionEventArgs> OnIntermediateConclusion;
        private List<Command> mCommands = new List<Command>();
        public List<Command> Commands
        {
            get
            {
                return mCommands;
            }
            set
            {
                if (mCommands != null)
                {
                    foreach(var m in mCommands)
                        m.OnIntermediateConclusion -= onIntermediateConclusion;
                }
                mCommands = value;
                if (mCommands != null)
                    foreach(var m in mCommands)
                        m.OnIntermediateConclusion += onIntermediateConclusion;
            }
        }

        private void onIntermediateConclusion(object sender, ConclusionEventArgs e)
        {
            OnIntermediateConclusion?.Invoke(this, new ConclusionEventArgs(e.Conclusion));
        }

        public Conclusion Execute()
        {
            if (Commands != null)
            {
                List<Conclusion> conclusions = new List<Conclusion>();
                foreach (var command in Commands)
                {
                    var result = command.Run();
                    if (result != null)
                    {
                        if (result is Conclusion)
                        {
                            if (result is HelpConclusion)
                                conclusions.Add(result.Execute());
                            else
                                conclusions.Add(result as Conclusion);
                        }
                        else
                            result.Execute();
                    }
                }
                if (conclusions.Count == 1)
                    return conclusions[0];
                else if (conclusions.Count == 0)
                    return new EmptyConclusion();
                else
                    return new MultiConclusions(conclusions);
            }
            else
                return new EmptyConclusion();
        }
        public Intention()
        {
            Commands = new List<Command>();
        }
    }
    public class Conclusion: Intention
    {

    }
    public class EmptyConclusion: Conclusion
    {

    }
    public class SuccessConclusion : Conclusion
    {
        public string Message { get; private set; }
        public SuccessConclusion(string message)
        {
            Message = message;
        }
    }
    public class HelpConclusion: Conclusion
    {

    }
    public class FailedConclusion : Conclusion
    {
        public string Message { get; private set; }
        public FailedConclusion(string message)
        {
            Message = message;
        }
    }
    public class SingeOutputConclusion: Conclusion
    {
        public string Output { get; private set; }
        public SingeOutputConclusion(string output)
        {
            Output = output;
        }
    }
    public class MultiOutputConclusion : Conclusion
    {
        
        private IEnumerable<object> ObjectList;
        private IEnumerable<string> OutputStrings;
        public MultiOutputConclusion(IEnumerable<string> outputs = null, IEnumerable<object> objects = null)
        {
            OutputStrings = outputs;
            ObjectList = objects;
        }
        
        public List<T> GetObjectList<T>()
        {
            if (ObjectList != null)
                return new List<T>(ObjectList.Cast<T>());
            else
                return null;
        }
        public List<string> GetOutputStrings()
        {
            if (OutputStrings != null)
                return new List<string>(OutputStrings);
            else
                return null;
        }
    }

    public class ExceptionConclusion: Conclusion
    {
        public Exception Exception { get; private set; }
        public ExceptionConclusion(Exception exp) => Exception = exp;        
    }
    public class MultiConclusions: Conclusion
    {
        private List<Conclusion> mConclusions;
        public MultiConclusions(List<Conclusion> conclusions) => mConclusions = conclusions;        
        public IEnumerable<Conclusion> Conclusions
        {
            get
            {
                if (mConclusions != null)
                    return mConclusions.AsEnumerable<Conclusion>();
                else
                    return null;
            }
        }
    }
    public class ConclusionEventArgs
    {
        public Conclusion Conclusion { get; private set; }
        public ConclusionEventArgs(Conclusion conclusion) => Conclusion = conclusion;
    }
}
