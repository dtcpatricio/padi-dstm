using System;

namespace Event
{
    // this delegate is the base for the sliders move event
    delegate void MoveEventHandler(object source, MoveEventArgs e);


    // this class contains the arguments of the slider move event
    public class MoveEventArgs : EventArgs
    {
        private int number;

        public MoveEventArgs(int number)
        {
            this.number = number;
        }

        public int Number
        {
            get { return number; }
        }

    }


    class Slider
    {
        private event MoveEventHandler moveEvent;
        private int position = 0;

        public event MoveEventHandler Event
        {
            add
            {
                moveEvent += value;
            }
            remove
            {
                if (moveEvent != null)
                {
                    moveEvent -= value;
                }
            }
        }

        public int Position
        {
            get
            {
                return position;
            }
            //this is what is run when we change the position
            set
            {
                if (value <= 50 && value >= 0)
                {
                    position = value;               
                }

                if (moveEvent != null)
                {
                    moveEvent(this, new MoveEventArgs(value));
                }
            }
        }

        public void currentPosition(Object sender, MoveEventArgs e)
        {
            Console.WriteLine("Posição actual: " + position);
        }
    }


    class Form
    {
        static void Main()
        {
            Slider slider = new Slider();

            // slider subscribes event, when event is triggered method slider_Move + currentPosition are executed
            slider.Event += slider_Move;
            slider.Event += slider.currentPosition;

            while (true)
            {
                Console.WriteLine("Insira uma nova posição (entre 0 e 50)");
                string input = Console.ReadLine();
                int num = Convert.ToInt32(input);
                slider.Position = num;
            }
            
            // these are two simulate slider moves
            // slider.Position = 20;
            // slider.Position = 60;
        }

        // this is the method that should be called when the slider is moved
        static void slider_Move(object source, MoveEventArgs e)
        {
            if (e.Number <= 50 && e.Number >= 0)
            {
                Console.WriteLine("Posição alterada para " + e.Number);
            }
            else
            {
                Console.WriteLine("Posição " + e.Number + " inválida");
            }
        }
    }
}