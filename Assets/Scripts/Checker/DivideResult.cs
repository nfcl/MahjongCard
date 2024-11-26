using Checker;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace Checker
{
    public enum BlockKind
    {
        Shun,
        Ke,
        Dui
    }
    public struct Block
    {
        public BlockKind kind;
        public int signCardKind;
        public int signCardNum;
        public bool isLastDrewCardExsist;

        public override string ToString()
        {
            return $"{BlockKindExtexsion.ToString(kind)} {signCardKind} {signCardNum}";
        }
    }
    public struct DivideResult
    {
        public List<Block> blocks;
        public bool hasHead;

        public Block queTou => blocks.Where(_ => _.kind == BlockKind.Dui).First();

        public DivideResult(DivideResult other)
        {
            blocks = new List<Block>(other.blocks);
            hasHead = other.hasHead;
        }
        public void Add(BlockKind blockKind, int cardKind, int cardNum, int num = 1)
        {
            if (blockKind == BlockKind.Dui)
            {
                hasHead = true;
            }
            for (int i = 0; i < num; ++i)
            {
                blocks.Add(new Block { kind = blockKind, signCardKind = cardKind, signCardNum = cardNum });
            }
        }
        public bool ExsistKeGroup(int kind, int num)
        {
            return blocks.Count(_ => _.kind == BlockKind.Ke && _.signCardKind == kind && _.signCardNum == num) != 0;
        }
        public bool ExsistShunGroup(int kind, int num)
        {
            return blocks.Count(_ => _.kind == BlockKind.Shun && _.signCardKind == kind && _.signCardNum <= num && num < _.signCardNum + 3) != 0;
        }
        public bool ExsistLiangMianShunGroup(int kind, int num)
        {
            return blocks.Count(_ => _.kind == BlockKind.Shun && _.signCardKind == kind && (num == _.signCardNum || num == _.signCardNum + 2)) != 0;
        }
        public bool ExsistDanMianShunGroup(int kind, int num)
        {
            return blocks.Count(_ => _.kind == BlockKind.Shun && _.signCardKind == kind && num == _.signCardNum + 1) != 0;
        }
        public bool ExsistCard(int kind, int num)
        {
            return blocks.Any(_ =>
                _.kind == BlockKind.Shun
                ? (_.signCardKind == kind && _.signCardNum <= num && num < _.signCardNum + 3)
                : (_.signCardKind == kind && _.signCardNum == num)
            );
        }
        public bool ExsistCards((int kind, int num)[] cards)
        {
            return blocks.Any(_ => cards.Any(__ =>
                _.kind == BlockKind.Shun
                ? (_.signCardKind == __.kind && _.signCardNum <= __.num && __.num < _.signCardNum + 3)
                : (_.signCardKind == __.kind && _.signCardNum == __.num)
            ));
        }
        public int[] GetExsistCardBlockIndex(int kind, int num)
        {
            return blocks
                .Select((_, index) => new { _, index })
                .Where(_ =>
                    _._.kind == BlockKind.Shun
                    ? (_._.signCardKind == kind && _._.signCardNum <= num && num < _._.signCardNum + 3)
                    : (_._.signCardKind == kind && _._.signCardNum == num
                ))
                .Select(_ => _.index)
                .ToArray();
        }
        public override string ToString()
        {
            return blocks.Aggregate("", (_, __) => $"{_}\n\t{__}");
        }
    }
}
public static class BlockKindExtexsion
{
    public static string ToString(BlockKind kind)
    {
        return kind switch
        {
            BlockKind.Dui => "对",
            BlockKind.Ke => "刻",
            BlockKind.Shun => "顺",
            _ => "?"
        };
    }
}