#pragma warning disable 0162
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using static Library;
using static Library.Piece;

public class Program
{
    static void Main(string[] args)
    {
        Piece[] pieces =
        {
            new Piece(new Vector[]{
                (1,1),
                (0,1),
                (1,0),
                (0,0)
            }),
            new Piece(new Vector[]{
                (0,2),
                (0,1),
                (2,0),
                (1,0),
                (0,0)
            }),
            new Piece(new Vector[]{
                (1,1),
                (4,0),
                (3,0),
                (2,0),
                (1,0),
                (0,0)
            }),
            new Piece(new Vector[]{
                (1,2),
                (0,2),
                (1,1),
                (0,1),
                (2,0),
                (1,0),
                (0,0)
            }),
            new Piece(new Vector[]{
                (3,5),
                (3,4),
                (2,4),
                (1,4),
                (2,3),
                (1,3),
                (0,3),
                (0,2),
                (0,1),
                (2,0),
                (1,0),
                (0,0)
            }),
            new Piece(new Vector[]{
                (5,1),
                (4,1),
                (3,1),
                (1,1),
                (0,1),
                (6,0),
                (5,0),
                (4,0),
                (3,0),
                (2,0),
                (1,0),
                (0,0)
            }),
            new Piece(new Vector[]{
                (2,2),
                (1,2),
                (0,2),
                (4,1),
                (3,1),
                (2,1),
                (1,1),
                (0,1),
                (4,0),
                (3,0),
                (2,0),
                (1,0),
            }),
            new Piece(new Vector[]{
                (5,2),
                (6,1),
                (5,1),
                (4,1),
                (3,1),
                (2,1),
                (1,1),
                (0,1),
                (5,0),
                (4,0),
                (3,0),
                (2,0),
                (1,0),
                (0,0)
            }),
            new Piece(new Vector[]{
                (2,2),
                (1,2),
                (0,2),
                (5,1),
                (3,1),
                (2,1),
                (1,1),
                (0,1),
                (5,0),
                (4,0),
                (3,0),
                (2,0),
                (1,0),
                (3,-1)
            }),
            new Piece(new Vector[]{
                (2,3),
                (1,3),
                (2,2),
                (1,2),
                (3,1),
                (2,1),
                (1,1),
                (0,1),
                (5,0),
                (4,0),
                (3,0),
                (2,0),
                (1,0),
                (0,0),
            })
        };
        foreach (Piece piece in pieces) piece.RefreshAllProperty();
        pieces = Reorder<Piece>(pieces, (left, right) =>
            right.Parts.Length > left.Parts.Length ||
            (right.Parts.Length == left.Parts.Length && right.SideLength > left.SideLength));

        bool[,] map = new bool[10, 10];
        Console.SetCursorPosition(0, 0);
        PuzzleSolver(map, pieces);
    }

    // /*
    private static void PuzzleSolver(bool[,] map, Piece[] pieces)
    {
        Vector sizeOfMap = (map.GetLength(0), map.GetLength(1));

        Console.WriteLine("////PROCESS START////");
        Data[] positions_D = new Data[pieces.Length];
        int[] positions = new int[pieces.Length];
        for (byte i = 0; i < pieces.Length; i++)
        {
            positions_D[i] = new Data("#" + i, "-1", alignDirection: Direction.Right, alignOffset: 10);
            positions[i] = -1;
            Console.Write("\n");
        }
        Thread ToggleUpdate = new Thread(() =>
        {
            for (; ; )
            {
                for (byte i = 0; i < positions.Length; i++) positions_D[i].Content = positions[i].ToString();
                Thread.Sleep(200);
            }
        });
        ToggleUpdate.Start();

        //set position to -Vector.one means request of reset position
        for (byte i = 0; i < pieces.Length; i++) pieces[i].Position = -Vector.one;

        List<PuzzleSolution> solutions = new List<PuzzleSolution>();
        byte currentPiece = 0;

        // process start
        for (; ; )
        {
            Piece piece = pieces[currentPiece];
            Vector UR_MoveRange, DL_MoveRange;
            UR_MoveRange = new Vector(sizeOfMap.x - 1, sizeOfMap.y - 1) - piece.UR_Boundary;
            DL_MoveRange = -piece.DL_Boundary;

            // Reset position
            if (piece.Position == -Vector.one) piece.Position = DL_MoveRange + Vector.down;

            // Move piece
            move:
            if (piece.Position.y < UR_MoveRange.y) piece.Position.y++;
            else if (piece.Position.x < UR_MoveRange.x)
            {
                piece.Position.y = DL_MoveRange.y;
                piece.Position.x++;
            }
            // tried all position, spin the piece and reset position
            else
            {
                if (currentPiece == 0) goto previous;
                piece.Position = -Vector.one;
                switch (piece.RotationalAngle)
                {
                    case 0:
                    case 4:
                        if (piece.RotationalSymmetry == Piece.RotationalSymmetryForm.RigleAngle) goto case 3;
                        goto case 2;
                    case 1:
                    case 5:
                        if (piece.RotationalSymmetry == Piece.RotationalSymmetryForm.FlatAngle) goto case 3;
                        goto case 2;
                    case 2:
                    case 6:
                        piece.RotationalAngle++;
                        positions[currentPiece]++;//
                        continue;
                    case 3:
                        if (piece.Symmetric || piece.RotationalAngle > 3) goto case 7;
                        goto case 2;
                    case 7:
                        piece.RotationalAngle = 0;
                        goto previous;
                }
            }
            positions[currentPiece]++;//

            // Check for overlap
            foreach (Vector vector in piece.Parts) if (map[vector.x + piece.Position.x, vector.y + piece.Position.y]) goto move;

            // check for remain space
            {
                bool[,] waterLayer = (bool[,])map.Clone();

                for (byte i = 0; i < piece.Parts.Length; i++) waterLayer[piece.Parts[i].x + piece.Position.x, piece.Parts[i].y + piece.Position.y] = true;

                List<byte> remainSpaces = new List<byte>();

                for (byte x = 0; x < sizeOfMap.x; x++)
                    for (byte y = 0; y < sizeOfMap.y; y++)
                    {
                        if (waterLayer[x, y]) continue;

                        List<Vector> waterList = new List<Vector>();
                        byte spaceCount = 1;
                        waterList.Add((x, y));
                        waterLayer[x, y] = true;

                        // 重複執行直到沒有空間
                        do
                        {
                            List<Vector> nextWater = new List<Vector>();
                            foreach (Vector water in waterList)
                            {
                                // 檢查四個方向
                                check(water + Vector.right);
                                check(water + Vector.left);
                                check(water + Vector.up);
                                check(water + Vector.down);

                                void check(Vector position)
                                {
                                    try
                                    {
                                        if (waterLayer[position.x, position.y]) return;
                                        waterLayer[position.x, position.y] = true;
                                        nextWater.Add((position.x, position.y));
                                        spaceCount++;
                                    }
                                    catch (IndexOutOfRangeException) { }
                                }
                            }
                            waterList = nextWater;
                        } while (waterList.Count != 0);
                        remainSpaces.Add(spaceCount);
                    }

                if (remainSpaces.Count > 1)
                {
                    // reorder
                    for (byte i = 0; i < remainSpaces.Count - 1; i++)
                    {
                        int indexOfMinimum = i;
                        for (int j = i + 1; j < remainSpaces.Count; j++)
                        {
                            if (remainSpaces[j] < remainSpaces[indexOfMinimum]) indexOfMinimum = j;
                        }
                        remainSpaces.Insert(i, remainSpaces[indexOfMinimum]);
                        remainSpaces.RemoveAt(indexOfMinimum + 1);
                    }

                    byte[] sizeOfPieces = new byte[pieces.Length];
                    for (byte i = 0; i < pieces.Length; i++) sizeOfPieces[i] = (byte)pieces[i].Parts.Length;

                    // 剩餘空間匹配檢查
                    foreach (byte space in remainSpaces)
                    {
                        List<byte> sizeOfPieces_i = new List<byte>(sizeOfPieces);
                        byte space_i = space;
                        sizeOfPieces_i.RemoveAll(x => x > space);

                        // 重新排列
                        for (byte i = 0; i < sizeOfPieces_i.Count - 1; i++)
                        {
                            byte indexOfGreatest = i;
                            for (byte j = (byte)(i + 1); j < sizeOfPieces_i.Count; j++)
                            {
                                if (sizeOfPieces_i[j] > sizeOfPieces_i[indexOfGreatest]) indexOfGreatest = j;
                            }
                            sizeOfPieces_i.Insert(i, sizeOfPieces_i[indexOfGreatest]);
                            sizeOfPieces_i.RemoveAt(indexOfGreatest + 1);
                        }

                        byte current = 0;
                        List<sbyte> solution = new List<sbyte>();
                        solution.Add((sbyte)-1);

                        // 檢查空間匹配
                        for (; ; )
                        {
                            // 處理當前位元

                            // 遞增至下一個值
                            if (solution[current] < sizeOfPieces_i.Count - 1) solution[current]++;
                            else// 沒有下一個值，回到上一個位元
                            {
                                if (current == 0) goto move;// 沒有上一個位元，傳回否

                                solution.RemoveAt(current);
                                current--;
                                space_i += sizeOfPieces_i[solution[current]];
                                continue;
                            }

                            // 檢查空間匹配
                            if (space_i < sizeOfPieces_i[solution[current]]) continue;// 空間不足，再遞增至下一個值
                            if (space_i == sizeOfPieces_i[solution[current]]) break;// 空間匹配，移至下一個項目

                            // 還有剩餘空間，前往下一位元
                            space_i -= sizeOfPieces_i[solution[current]];
                            solution.Add(solution[current]);
                            current++;
                        }
                    }
                }
            }
            WriteMap(piece, true);

            // Record Solution
            if (currentPiece == pieces.Length - 1)
            {
                (byte ID, Vector position, byte rotationalAngle)[] solution = new (byte ID, Vector position, byte rotationalAngle)[pieces.Length];
                for (byte i = 0; i < pieces.Length; i++) solution[i] = (i, pieces[i].Position, pieces[i].RotationalAngle);
                solutions.Add(solution);

                WriteMap(piece, false);
                goto previous;
            }
            currentPiece++;
            continue;

        // Back to prevous piece
        previous:
            if (currentPiece == 0) break;
            piece.Position = -Vector.one;
            piece.RotationalAngle = 0;
            positions[currentPiece] = -1;//
            currentPiece--;
            WriteMap(pieces[currentPiece], false);
            void WriteMap(Piece piece, bool value)
            {
                for (byte i = 0; i < piece.Parts.Length; i++) map[piece.Parts[i].x + piece.Position.x, piece.Parts[i].y + piece.Position.y] = value;
            }
        }

        Console.WriteLine($"solutions found: {solutions.Count}");//
    }
    // */
}