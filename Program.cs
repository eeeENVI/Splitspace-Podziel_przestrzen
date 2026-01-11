using System;

try {
    using var game = new Splitspace_Podziel_przestrzen.Game1();
    game.Run();
} catch (Exception e) {
    Console.WriteLine(e.ToString());
    throw;
}

