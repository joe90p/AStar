using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace AStar
{
    class Program
    {
        static void Main(string[] args)
        {
            Problem problem = new Problem();
            problem.Solve();
        }
    }

    public class BoardComparer : IEqualityComparer<Board>
    {
        public bool Equals(Board x, Board y)
        {
            return x.StateAsString.Equals(y.StateAsString);
        }
        public int GetHashCode(Board obj)
        {

            return obj.StateAsString.GetHashCode();

        }
    }

    public class Action
    {
        public string ResultState
        {
            get;
            private set;
        }

        public int Cost
        {
            get;
            private set;
        }

        public Action(string resultState, int cost)
        {
            this.ResultState = resultState;
            this.Cost = cost;
        }
    }



    public class Board 
    {

        private string stateAsString;
        private int size;

        public string StateAsString
        {
            get { return this.stateAsString; }
        }

        public Board(int[][] initialStateAsArray, int size)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    sb.Append(initialStateAsArray[i][j]);
                }
            }
            this.stateAsString = sb.ToString();
            this.size = size;
        }

        public Board(string state, int size)
        {
            this.stateAsString = state;
            this.size = size;
        }

        private int GetZeroPosition()
        {
            return stateAsString.IndexOf('0');
        }

        private int GetManhattenDistanceChangeFromSwap(Point char1GoalPosition, char character2, Point oldChar1Position, int newchar1Index)
        {
            Point char2GoalPosition = this.GetPositionInGoal(character2);
            Point newChar1Position = this.GetPositionFromIndex(newchar1Index);
            int oldchar1Distance = this.GetManhattenDistanceFromGoal(char1GoalPosition, oldChar1Position);
            int newchar1Distance = this.GetManhattenDistanceFromGoal(char1GoalPosition, newChar1Position);
            int oldchar2Distance = this.GetManhattenDistanceFromGoal(char2GoalPosition, newChar1Position);
            int newchar2Distance = this.GetManhattenDistanceFromGoal(char2GoalPosition, oldChar1Position);
            return (newchar1Distance - oldchar1Distance) + (newchar2Distance - oldchar2Distance);
        }

        private int GetManhattenDistanceFromGoal(Point goalPosition, Point currentPosition)
        {
            int manhattenDistance = (goalPosition.X - currentPosition.X) + (goalPosition.Y - currentPosition.Y);
            return Math.Abs(manhattenDistance);
        }

        private Point GetPositionInGoal(char c)
        {
            int indexInGoal = Int32.Parse(c.ToString());
            return this.GetPositionFromIndex(indexInGoal);
        }

        private Point GetPositionFromIndex(int i)
        {
            int row = i / this.size;
            int column = i < this.size ? i : i % this.size;
            return new Point(column, row);
        }

        public List<Action> GetActionResults()
        {
            List<Action> actionResults = new List<Action>();
            int zeroIndex = this.GetZeroPosition();
            Point currentZeroPosition = this.GetPositionFromIndex(zeroIndex);
            Point zeroGoalPosition = this.GetPositionInGoal('0');

            int positionAbove = zeroIndex - this.size;
            int positionBelow = zeroIndex + this.size;
            int positionLeft = zeroIndex - 1;
            int positionRight = zeroIndex + 1;

            bool hasPositionAbove = positionAbove >=0;
            bool hasPositionBelow = positionBelow < (this.size * this.size);
            bool hasPositionLeft = positionLeft >=0 && (positionLeft / this.size == currentZeroPosition.Y);
            bool hasPositionRight = positionRight / this.size == currentZeroPosition.Y;

            if (hasPositionAbove)
            {
                string result = SwapCharacters(this.stateAsString, zeroIndex, positionAbove);
                int cost = this.GetManhattenDistanceChangeFromSwap(zeroGoalPosition, stateAsString[positionAbove], currentZeroPosition, positionAbove);
                actionResults.Add(new Action(result, cost));
            }

            if (hasPositionBelow)
            {
                string result = SwapCharacters(this.stateAsString, zeroIndex, positionBelow);
                int cost = this.GetManhattenDistanceChangeFromSwap(zeroGoalPosition, stateAsString[positionBelow], currentZeroPosition, positionBelow);
                actionResults.Add(new Action(result, cost));
            }

            if (hasPositionLeft)
            {
                string result = SwapCharacters(this.stateAsString, zeroIndex, positionLeft);
                int cost = this.GetManhattenDistanceChangeFromSwap(zeroGoalPosition, stateAsString[positionLeft], currentZeroPosition, positionLeft);
                actionResults.Add(new Action(result, cost));
            }

            if (hasPositionRight)
            {
                string result = SwapCharacters(this.stateAsString, zeroIndex, positionRight);
                int cost = this.GetManhattenDistanceChangeFromSwap(zeroGoalPosition, stateAsString[positionRight], currentZeroPosition, positionRight);
                actionResults.Add(new Action(result, cost));
            }

            return actionResults;
        }

        static string SwapCharacters(string value, int position1, int position2)
        {
            char[] array = value.ToCharArray(); 
            char temp = array[position1]; 
            array[position1] = array[position2]; 
            array[position2] = temp; 
            return new string(array); 
        }

        public int GetNumberMisplacedBlocksDistanceSum()
        {
            int distanceSum = 0;
            for (int k = 0; k < this.StateAsString.Length; k++)
            {
                Point goalPosition = this.GetPositionInGoal(this.StateAsString[k]);
                Point currentPosition = this.GetPositionFromIndex(k);
                distanceSum += this.GetManhattenDistanceFromGoal(goalPosition, currentPosition);
            }
            return distanceSum;
        }

        public bool IsGoal()
        {
            return this.StateAsString.Equals("012345678");
        }

        public void Print()
        {
            for (int i = 0; i < this.size; i++)
            {
                Console.WriteLine(this.stateAsString.Substring((i*this.size), this.size));
            }
            Console.WriteLine();
        }
    }

    public class Problem
    {
        int[][] initialstate = new int[3][];

        public Problem()
        {
            this.initialstate[0] = new int[3] { 3,8,6 };
            this.initialstate[1] = new int[3] { 5,7,1 };
            this.initialstate[2] = new int[3] { 2,4,0 };

        }

        public void Solve()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            int size = 3;
            //List<Path> pathsToExplore = new List<Path>();
            PriorityQueueDemo.PriorityQueue<int, Node> nodestoExplore = new PriorityQueueDemo.PriorityQueue<int, Node>();
            //List<Board> exploredStates = new List<Board>();
            Dictionary<string, Board> exploredStates = new Dictionary<string, Board>();


            Board initialBoard =  new Board(initialstate, size);
            int initialCost = initialBoard.GetNumberMisplacedBlocksDistanceSum();
            Node intialPath = new Node(
                                                    initialBoard,
                                                    null,
                                                    initialCost
                                                    );

            nodestoExplore.Enqueue(initialCost, intialPath, intialPath.EndState.StateAsString);

            Node currentPath = new Node(null);

            while (true)
            {
                //pathsToExplore = pathsToExplore.OrderBy(x => x.Cost).ToList();


                currentPath = nodestoExplore.DequeueValue(); //pathsToExplore[0];
                nodestoExplore.DeleteFromStateDictionary(currentPath.EndState.StateAsString);
                //pathsToExplore.RemoveAt(0);
                //var frontier = from p in nodestoExplore
                 //              select p.Value.EndState;
                Board currentState = currentPath.EndState;

                exploredStates.Add(currentState.StateAsString, currentState);
                if (currentState.IsGoal())
                {
                    break;
                }
                foreach (Action action in currentState.GetActionResults())
                {
                    if (
                            !exploredStates.ContainsKey(action.ResultState)
                            &&
                            !nodestoExplore.ContainsState(action.ResultState)

                        )
                    {
                        int cost = currentPath.Cost + action.Cost + 1;
                        Node pathToAdd = new Node(
                                                    new Board(action.ResultState, size),
                                                    currentPath,
                                                    cost
                                                    );
                        
                        nodestoExplore.Enqueue(cost, pathToAdd, pathToAdd.EndState.StateAsString);
                        
                    }

                }
            }
            stopWatch.Stop();
            currentPath.Print();
            Console.ReadKey();
        }
    }

    class Node
    {
        private Node parent;
        private int cost = 0;

        public int Cost
        {
            get { return this.cost; }
        }
        public Board EndState
        {
            get;
            private set;

        }

        public Node(Board endState)
        {
            this.EndState = endState;
        }

        public Node(Board endState, Node parent)
            : this(endState)
        {
            this.parent = parent;
        }

        public Node(Board endState, Node parent, int heuristicCost)
            : this(endState, parent)
        {
            this.cost = heuristicCost;
        }

        public void Print()
        {
            this.EndState.Print();
            if (parent != null)
            {
                parent.Print();
            }
        }

    }

    class Point
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
