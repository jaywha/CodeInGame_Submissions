using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Player
{
    static void Main(string[] args)
    {
        int playerIdx = int.Parse(Console.ReadLine());
        int nbGames = int.Parse(Console.ReadLine());

        TrackModel trackGame = null;
        bool breakOutTheGameLoop = false;

        // game loop
        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                string scoreInfo = Console.ReadLine();
            }
            for (int i = 0; i < nbGames; i++)
            {
                string[] inputs = Console.ReadLine().Split(' ');
                string gpu = inputs[0];                
                int reg0 = int.Parse(inputs[1]);
                int reg1 = int.Parse(inputs[2]);
                int reg2 = int.Parse(inputs[3]);
                int reg3 = int.Parse(inputs[4]);
                int reg4 = int.Parse(inputs[5]);
                int reg5 = int.Parse(inputs[6]);
                int reg6 = int.Parse(inputs[7]);
                // Console.Error.WriteLine(GameReport(gpu, reg0, reg1, reg2, reg3, reg4, reg5, reg6));

                if (gpu == "GAME_OVER") {
                    trackGame = null;
                    Console.WriteLine("DOWN"); // ignored
                    breakOutTheGameLoop = true;
                } else if (i == 0 && trackGame == null) {
                    trackGame = new TrackModel(gpu);
                }
            }

            if (breakOutTheGameLoop) {
                breakOutTheGameLoop = false;
                continue;
            }

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            // 1. Track Game
            // Console.Error.WriteLine("Hurdle Indexes: (" + string.Join(", ", trackGame.hurdleQueue) + ")");
            // Console.Error.WriteLine($"Next Hurdle: {trackGame.nextHurdle}");
            int spaceToNextHurdle = trackGame.nextHurdle;
            if (spaceToNextHurdle > 3) {
                Console.WriteLine("RIGHT");
                trackGame.move(3);
            } else if (spaceToNextHurdle == 3) {
                Console.WriteLine("DOWN");
                trackGame.move(2);
            } else if (spaceToNextHurdle == 2) {
                Console.WriteLine("LEFT");
                trackGame.move(1);
            } else if (spaceToNextHurdle == 1) { // there's a hurdle, so JUMP!
                Console.WriteLine("UP");
                trackGame.getNextHurdle();
                trackGame.move(1);
            } else {
                Console.WriteLine("RIGHT");
            }
        }
    }

    public static string GameReport(string gpu, int reg0, int reg1, int reg2, 
    int reg3, int reg4, int reg5, int reg6) {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(new string('=', 10));
        sb.AppendLine("Track Game");
        sb.AppendLine("GPU: " + gpu);
        sb.AppendLine("\tReg 0: " + reg0);
        sb.AppendLine("\tReg 1: " + reg1);
        sb.AppendLine("\tReg 2: " + reg2);
        sb.AppendLine("\tReg 3: " + reg3);
        sb.AppendLine("\tReg 4: " + reg4);
        sb.AppendLine("\tReg 5: " + reg5);
        sb.AppendLine("\tReg 6: " + reg6);
        sb.AppendLine(new string('=', 10));
        return sb.ToString();
    }

    class TrackModel {
        public Queue<int> hurdleQueue = new Queue<int>();
        public int nextHurdle = -1;

        public TrackModel(string track) {
            int prevHindex = 0;         
            int hindex = track.IndexOf('#');
            while (hindex > -1) {
                hurdleQueue.Enqueue(hindex - prevHindex);
                prevHindex = hindex;
                hindex = track.IndexOf('#', hindex+1);
            }

            hurdleQueue.Reverse();
            getNextHurdle();
        }

        public void getNextHurdle() {
            if (!hurdleQueue.TryDequeue(out nextHurdle)) {
                nextHurdle = -1;
            }
        }

        public void move(int space) {
            nextHurdle -= space;
        }
    }
}