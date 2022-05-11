
using System.Collections.Generic;

public interface IPawnable
{
    int MoveDistance { get; set; }
    int AttacksLeft { get; set; }
    Tile CurrentTile { get; set; }
    List<Tile> GetAvailableMoves();

    void MoveTo(Tile destination);

    void Attack();



}
