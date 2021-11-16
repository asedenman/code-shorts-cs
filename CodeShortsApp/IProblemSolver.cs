// ----------------------------------------------------------------------------------------------------------------
// ---------------------------- CodeShorts ----------------------------
// ---------------------------- CodeShortsApp ----------------------------
// ----------------------------------------------------------------------------------------------------------------
// 
// File: IProblemSolver.cs
// Created: 09/11/2021
// By: ASED
// 
// ----------------------------------------------------------------------------------------------------------------

namespace CodeShortsApp
{
    public interface IProblemSolver
    {
        public abstract string Solve();

        public abstract void LoadData();
    }
}