using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinder
{
    class Timer
    {
        //time storage
        public float time = 0;
        //start tick
        long singleTick = 0;
        //is paused
        bool pauseF = false;
        //has started
        bool startF = false;
        public Timer()
        {
            
        }
        //begin the timer
        public void start()
        {
            startF = true;
            pauseF = false;
            time = 0;
        }
        //pause and unpause the timer
        public void pause()
        {
            pauseF = !pauseF;
        }
        //stop the timer
        public void stop()
        {
            startF = false;
            pauseF = false;
        }
        //update
        public void update(float deltaTime)
        {
            if(startF && pauseF)
            time += deltaTime;
        }
        //start tick timer
        public void startTick()
        {
            singleTick = DateTime.Now.Ticks;
        }
        //get the ticks past since startTick
        public float getTick()
        {
            return (DateTime.Now.Ticks - singleTick)/1000;
        }
    }
}
