﻿namespace Evolution;

public interface ICrossover
{
    public Chromosome Crossover(Chromosome parentA, Chromosome parentB);
}
