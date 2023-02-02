using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    private Vector2Int _fieldSize;

    [SerializeField]
    private List<Cell> _cells;

    private List<List<Cell>> _board;

    public List<List<Cell>> GetBoard() => _board;

    public void Init()
    {
        BoardCreator creator = new BoardCreator(_fieldSize, this);
        BoardPosMover mover = new(this);

        _board = creator.GetBoard();
        mover.MoveEveryCellOnItsPos();
    }

    public Cell GetRandomItem()
    {
        Cell item = Instantiate(_cells[Random.Range(0, _cells.Count)]);
        item.transform.parent = transform;
        item.transform.localPosition = Vector3.zero;
        return item;
    }

    private void Start()
    {
        Init();
    }
}

public class BoardPosMover
{
    private List<List<Cell>> _cells => _board.GetBoard();
    private Board _board;

    public BoardPosMover(Board board)
    {
        _board = board;
    }

    public IEnumerator StartMoving()
    {
        IEnumerator MoveAtPos(Transform cell, Vector3 pos)
        {
            float usedTime = 0f;
            yield return new WaitForSeconds(Time.deltaTime);

            while (cell.localPosition != pos)
            {
                usedTime += Time.deltaTime;
                cell.localPosition = Vector2.Lerp(cell.localPosition, pos, usedTime / 1f);
                yield return new WaitForFixedUpdate();
            }
        }

        Vector2 Pos = Vector2.zero;
        float Step = 1.5f;

        foreach (var row in _cells)
        {
            foreach (var cell in row)
            {
                _board.StartCoroutine(MoveAtPos(cell.transform, Pos));
                Pos += Vector2.right * Step;
                
                yield return new WaitForSeconds(0.05f);
            }

            Pos.x = 0;
            Pos += Vector2.up * Step;
        }
    }

    public void MoveEveryCellOnItsPos()
    {
        _board.StartCoroutine(StartMoving());
    }
}

public class BoardCreator
{
    private Vector2Int _size;
    private Board _board;

    public BoardCreator(Vector2Int size, Board board)
    {
        _board = board;
        _size = size;
    }

    public List<List<Cell>> GetBoard()
    {
        List<List<Cell>> result = new();

        for (int i = 0; i < _size.y; i++)
        {
            List<Cell> row = new();

            for (int j = 0; j < _size.x; j++)
            {
                row.Add(_board.GetRandomItem());
            }

            result.Add(row);
        }

        return result;
    }
}
