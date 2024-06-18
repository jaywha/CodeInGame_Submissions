using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int R = int.Parse(inputs[0]); // number of rows.
        int C = int.Parse(inputs[1]); // number of columns.
        int A = int.Parse(inputs[2]); // number of rounds between the time the alarm countdown is activated and the time the alarm goes off.

        Map map = new Map(R);
        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int KR = int.Parse(inputs[0]); // row where Rick is located.
            int KC = int.Parse(inputs[1]); // column where Rick is located.
            for (int i = 0; i < R; i++)
            {
                string ROW = Console.ReadLine(); // C of the characters in '#.TC?' (i.e. one line of the ASCII maze).
                map.UpdateRow(i, ROW);
            }

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            Console.Error.WriteLine(new string('=', 10));
            Console.Error.WriteLine(map);

            // Rick's next move (UP DOWN LEFT or RIGHT).
            // #1: Always Move Right
            if (map.GetCell(KR, KC+1) != (char) CellTypes.Wall)
                Console.WriteLine("RIGHT");
            else if (map.GetCell(KR+1, KC) != (char) CellTypes.Wall) {
                Console.WriteLine("DOWN");
            } else if (map.GetCell(KR, KC-1) != (char) CellTypes.Wall) {
                Console.WriteLine("LEFT");
            } else if (map.GetCell(KR-1, KC) != (char) CellTypes.Wall) {
                Console.WriteLine("UP");
            }
        }
    }

    enum CellTypes {
        Wall = '#',
        Space = '.',
        Starting = 'T',
        Control = 'C'        
    }

    class Map {
        Dictionary<int, string> map;
        (int, int) startingPoint;
        (int, int) controlPanel;
        List<(int, int)> visited;

        public Map(int rowNum) {
            map = new(rowNum);
            visited = new();
        }

        public char GetCell(int row, int col) {
            if (map == null || map.Count == 0) {
                return ' ';
            }

            var cell = map[row][col];
            if (cell == (char) CellTypes.Starting) {
                startingPoint = (row, col);
            }

            if (cell == (char) CellTypes.Control) {
                controlPanel = (row, col);
            }

            return cell;
        }

        public void UpdateRow(int index, string row) {
            if (index < 0 || map == null || index > map.Count) {
                return;
            }

            map[index] = row;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            foreach(var kvp in map) {
                sb.AppendLine(kvp.Value);
            }
            return sb.ToString();
        }
    }
}