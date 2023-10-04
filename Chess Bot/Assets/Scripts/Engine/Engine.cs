using System.Collections.Generic;
using UnityEngine;
using static PieceSquareTables;
using System.Linq;
using JetBrains.Annotations;

public static class Engine
{
    public static TranspositionTable transpositionTable;

    public static int searchDepth = 1;
    public static int tableSizeMB = 8;

    private static int friendlyColor = -1;
    private static long positionEvaluated = 0;

    const int pawnValue = 100;
    const int knightValue = 300;
    const int bishopValue = 320;
    const int rookValue = 500;
    const int queenValue = 900;
    const int kingValue = 12000;

    public static void Search()
    {
        Log.Message("Started search...");
        Log.Message("Search depth: " + searchDepth);

        List<Move> possibleMoves = MoveGenerator.GenerateMoves();
        possibleMoves = ValidateMoves(possibleMoves);

        /*for (int depth = 1; depth < 6; depth++)
        {

            AlphaBeta(possibleMoves, depth, int.MinValue, int.MaxValue, Board.GetWhiteToMove());
            Debug.Log("Depth " + depth + ": Positions evaluated: " + positionEvaluated);
            positionEvaluated = 0;
        }*/

        Debug.Log("Alpha Beta Search: " + AlphaBeta(possibleMoves, searchDepth, int.MinValue, int.MaxValue, Board.GetWhiteToMove()));

        Debug.Log("Depth " + searchDepth + ": Positions evaluated: " + positionEvaluated);
    }

    public static List<Move> ValidateMoves(List<Move> possibleMoves)
    {
        friendlyColor = Board.GetWhiteToMove() ? Piece.WHITE : Piece.BLACK;

        List<Move> illegalMoves = new();

        int prevKingSq = -1;

        foreach (Move move in possibleMoves)
        {
            Board.MakeMove(move, true);

            int kingSq = GetKingSquare(prevKingSq);

            if (move.StartSquare == kingSq) kingSq = move.StartSquare;
            
            List<Move> responses = MoveGenerator.GenerateMoves();
            
            foreach (Move response in responses)
            {
                if (move.TargetSquare == kingSq)
                {
                    illegalMoves.Add(move);
                }
            }

            Board.UnmakeMove(move, true);
        }

        possibleMoves.RemoveAll(move => illegalMoves.Contains(move));

        return possibleMoves;
    }

    private static int GetKingSquare(int prevKingSq)
    {
        ulong kingBitboard = Board.GetBitboards()[Piece.KING | friendlyColor];

        if (prevKingSq != -1)
        {
            ulong prevMask = 1ul << prevKingSq;
            if ((kingBitboard & prevMask) != 0) return prevKingSq;
        }

        ulong mask = 1ul;

        for (int sq = 0; sq < 64; sq++)
        {
            if ((kingBitboard & mask) != 0)
            {
                return sq;
            }
            mask <<= 1;
        }

        Debug.LogWarning("King bitboard doesn't contain a position for the king");
        return -1;
    }

    public static int AlphaBeta(List<Move> possibleMoves, int depth, int alpha, int beta, bool isWhite)
    {
        if (depth == 0 || possibleMoves.Count == 0)
            return Evaluate(depth);

        if (isWhite)
        {
            int maxEvaluation = int.MinValue;
            foreach (Move move in possibleMoves)
            {
                Board.MakeMove(move, true);
                int evaluation = AlphaBeta(MoveGenerator.GenerateMoves(), depth - 1, alpha, beta, false);
                Board.UnmakeMove(move, true);

                if (evaluation > maxEvaluation) maxEvaluation = evaluation;
                if (alpha > evaluation) alpha = evaluation;
                if (beta <= alpha) break;
            }
            return maxEvaluation;
        }
        else
        {
            int minEvaluation = int.MaxValue;
            foreach (Move move in possibleMoves)
            {
                Board.MakeMove(move, true);
                int evaluation = AlphaBeta(MoveGenerator.GenerateMoves(), depth - 1, alpha, beta, true);
                Board.UnmakeMove(move, true);

                if (evaluation < minEvaluation) minEvaluation = evaluation;
                if (beta < evaluation) beta = evaluation;
                if (beta <= alpha) break;
            }
            return minEvaluation;
        }
    }

    public static int Evaluate(int depth = -1)
    {
        PositionState currentPositionState = Board.GetCurrentPositionState();
        Entry entry = transpositionTable.Lookup(currentPositionState.zobristKey);

        if (entry != null) return entry.Evaluation;

        int evaluation = 0;
        int whiteEvaluation = 0;
        int blackEvaluation = 0;
        ulong[] bitboards = Board.GetBitboards();

        int[] sides = { Piece.WHITE, Piece.BLACK };

        foreach (int color in sides)
        {
            // Positive Evaluation bedeutet Vorteil fuer Weiss, negativ fuer Schwarz.
            int colorMultiplier = (color == Piece.WHITE) ? 1 : -1;

            for (int pieceType = color + 1; pieceType < color + 7; pieceType++)
            {
                ulong bitboard = bitboards[pieceType];
                if (bitboard == 0) continue;

                var piecePositions = GetPiecePositions(bitboard);
                foreach (int square in piecePositions)
                {
                    evaluation += EvaluatePiecePosition(square, color, pieceType);
                }

                int count = piecePositions.Count();

                if (Piece.IsType(pieceType, Piece.KING)) evaluation += kingValue;
                else if (Piece.IsType(pieceType, Piece.PAWN)) evaluation += count * pawnValue;
                else if (Piece.IsType(pieceType, Piece.KNIGHT)) evaluation += count * knightValue;
                else if (Piece.IsType(pieceType, Piece.BISHOP)) evaluation += count * bishopValue;
                else if (Piece.IsType(pieceType, Piece.ROOK)) evaluation += count * rookValue;
                else if (Piece.IsType(pieceType, Piece.QUEEN)) evaluation += count * queenValue;
                
            }
            whiteEvaluation += color == Piece.WHITE ? evaluation : 0;
            blackEvaluation += color == Piece.BLACK ? evaluation : 0;
            evaluation = 0;
        }
        //Debug.Log("White " + whiteEvaluation + " Black " + blackEvaluation);

        positionEvaluated++;
        evaluation = whiteEvaluation - blackEvaluation;

        transpositionTable.Store(currentPositionState.zobristKey, evaluation, depth);

        return evaluation;
    }

    private static int EvaluatePiecePosition(int square, int color, int pieceType)
    {
        if (Piece.IsType(pieceType, Piece.KING))
            return color == Piece.WHITE ? WhiteKingValues[square] : BlackKingValues[square];
        else if (Piece.IsType(pieceType, Piece.PAWN))
            return color == Piece.WHITE ? WhitePawnValues[square] : BlackPawnValues[square];
        else if (Piece.IsType(pieceType, Piece.KNIGHT))
            return color == Piece.WHITE ? WhiteKnightValues[square] : BlackKnightValues[square];
        else if (Piece.IsType(pieceType, Piece.BISHOP))
            return color == Piece.WHITE ? WhiteBishopValues[square] : BlackBishopValues[square];
        else if (Piece.IsType(pieceType, Piece.ROOK))
            return color == Piece.WHITE ? WhiteRookValues[square] : BlackRookValues[square];
        else if (Piece.IsType(pieceType, Piece.QUEEN))
            return color == Piece.WHITE ? WhiteQueenValues[square] : BlackQueenValues[square];
        else
        {
            Debug.LogError("Couldn't find a pieceType for the given piece");
            return 0;
        }
    }

    public static IEnumerable<int> GetPiecePositions(ulong bitboard)
    {
        for (int i = 0; i < 64; i++)
        {
            if (((bitboard >> i) & 1UL) == 1UL)
            {
                yield return i;
            }
        }
    }

    public static void Initialize()
    {
        int tableSize = 1024 * 1024 * tableSizeMB;

        transpositionTable = new TranspositionTable(tableSize);
    }
}
