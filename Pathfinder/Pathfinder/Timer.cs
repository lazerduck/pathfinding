using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinder
{
    class Timer
    {
        public float time = 0;
        long singleTick = 0;
        bool pauseF = false;
        bool startF = false;
        public Timer()
        {
            
        }
        public void start()
        {
            startF = true;
            pauseF = false;
            time = 0;
        }
        public void pause()
        {
            pauseF = true;
        }
        public void stop()
        {
            startF = false;
            pauseF = false;
        }
        public void update(float deltaTime)
        {
            if(startF && pauseF)
            time += deltaTime;
        }
        public void startTick()
        {
            singleTick = DateTime.Now.Ticks;
        }
        public float getTick()
        {
            return (DateTime.Now.Ticks - singleTick)/1000;
        }
    }
}
