// <copyright file="Program.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

using System.Diagnostics;
using ChecksumCalculator;

if (args.Length == 0)
{
    return;
}

var directoryPath = args[0];

if (!Directory.Exists(directoryPath))
{
    Console.WriteLine($"Error: directory not found: {directoryPath}");
    return;
}

var sequential = new SequentialChecksumCalc();
var parallel = new ParallelChecksumCalc();

Console.WriteLine($"Calculating checksum for: {Path.GetFullPath(directoryPath)}");
Console.WriteLine();

var sw = Stopwatch.StartNew();
var hashSeq = await sequential.ComputeChecksumBase64Async(directoryPath);
sw.Stop();
var timeSeq = sw.Elapsed.TotalSeconds;

sw.Restart();
var hashPar = await parallel.ComputeChecksumBase64Async(directoryPath);
sw.Stop();
var timePar = sw.Elapsed.TotalSeconds;

Console.WriteLine($"Sequential → {hashSeq}   ({timeSeq:F3}s)");
Console.WriteLine($"Parallel   → {hashPar}   ({timePar:F3}s)");
Console.WriteLine();

Console.WriteLine(hashSeq == hashPar ? "Success: Hashes are identical" : "Failure: Hashes differ — something went wrong!");