﻿using Language.CodeAnalysis.Symbols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Language.CodeAnalysis.Binding
{
    internal sealed class ControlFlowGraph
    {
        private ControlFlowGraph(BasicBlock start, BasicBlock end, List<BasicBlock> blocks, List<BasicBlockBranch> branches) 
        {
            Start = start;
            End = end;
            Blocks = blocks;
            Branches = branches;
        }

        public BasicBlock Start { get; }
        public BasicBlock End { get; }
        public List<BasicBlock> Blocks { get; }
        public List<BasicBlockBranch> Branches { get; }

        public sealed class BasicBlock
        {
            public BasicBlock()
            {

            }

            public BasicBlock(bool isStart)
            {
                IsStart = isStart;
                IsEnd = !isStart;
            }

            public bool IsStart { get; }
            public bool IsEnd { get; }
            public List<BoundStatement> Statements { get; } = new();
            public List<BasicBlockBranch> Incoming { get; } = new();
            public List<BasicBlockBranch> Outgoing { get; } = new();

            public override string ToString()
            {
                if (IsStart)
                    return "<Start>";
                if (IsEnd)
                    return "<End>";

                using(var stringWriter = new StringWriter())
                {
                    foreach(var statement in Statements)
                        statement.WriteTo(stringWriter);

                    return stringWriter.ToString();
                }
                
            }
        }

        public sealed class BasicBlockBranch
        {
            public BasicBlockBranch(BasicBlock from, BasicBlock to, BoundExpression condition)
            {
                From = from;
                To = to;
                Condition = condition;
            }

            public BasicBlock From { get; }
            public BasicBlock To { get; }
            public BoundExpression Condition { get; }

            public override string ToString()
            {
                if(Condition == null)
                    return string.Empty;

                return Condition.ToString();
            }
        }

        public sealed class BasicBlockBuilder
        {
            private List<BoundStatement> _statements = new();
            private List<BasicBlock> _blocks = new();

            public List<BasicBlock> Build(BoundBlockStatement block)
            {
                foreach(var statement in block.Statements)
                {
                    switch (statement.Kind)
                    {

                        case BoundNodeKind.LabelStatement:
                            StartBlock();
                            _statements.Add(statement);
                            break;
                        case BoundNodeKind.GotoStatement:
                        case BoundNodeKind.ConditionalGotoStatement:
                        case BoundNodeKind.ReturnStatement:
                            _statements.Add(statement);
                            StartBlock();
                            break;
                        case BoundNodeKind.VariableDeclaration:
                        case BoundNodeKind.ExpressionStatement:
                            _statements.Add(statement);
                            break;
                        default:
                            throw new Exception($"Unexpected statement: {statement.Kind}");
                    }
                }

                EndBlock();

                return _blocks.ToList();
            }

            private void StartBlock()
            {
                EndBlock();
            }

            private void EndBlock()
            {
                if(_statements.Count > 0)
                {
                    var block = new BasicBlock();

                    block.Statements.AddRange(_statements);
                    _blocks.Add(block);
                    _statements.Clear();
                }
            }

        }

        public sealed class GraphBuilder
        {
            private Dictionary<BoundStatement, BasicBlock> _blockFromStatement = new();
            private Dictionary<BoundLabel, BasicBlock> _blockFromLabel = new();
            private List<BasicBlockBranch> _branches = new();
            private BasicBlock _start = new BasicBlock(isStart: true);
            private BasicBlock _end = new BasicBlock(isStart: false);

            public ControlFlowGraph Build(List<BasicBlock> blocks)
            { 
                if (!blocks.Any())
                    Connect(_start, _end);
                else
                    Connect(_start, blocks.First());

                foreach(var block in blocks)
                {
                    foreach (var statement in block.Statements)
                    {
                        _blockFromStatement.Add(statement, block);

                        if(statement is BoundLabelStatement labelStatement)
                            _blockFromLabel.Add(labelStatement.Label, block);
                    }
                }

                for (int i = 0; i < blocks.Count; i++)
                {
                    var current = blocks[i];
                    var next = i == blocks.Count - 1 ? _end : blocks[i + 1];

                    foreach (var statement in current.Statements)
                    {
                        var isLastStatementInBlock = statement == current.Statements.Last();

                        switch (statement.Kind)
                        {
                            case BoundNodeKind.GotoStatement:
                                var gotoStatement = (BoundGotoStatement)statement;
                                var toBlock = _blockFromLabel[gotoStatement.Label];
                                Connect(current, toBlock);
                                break;
                            case BoundNodeKind.ConditionalGotoStatement:
                                var conditionalGotoStatement = (BoundConditionalGotoStatement)statement;
                                var thenBlock = _blockFromLabel[conditionalGotoStatement.Label];
                                var elseBlock = next;
                                var negatedCondition = Negate(conditionalGotoStatement.Condition);
                                var thencondition = conditionalGotoStatement.JumpIfTrue ? conditionalGotoStatement.Condition : negatedCondition;
                                var elseCondition = conditionalGotoStatement.JumpIfTrue ? negatedCondition : conditionalGotoStatement.Condition;

                                Connect(current, thenBlock, thencondition);
                                Connect(current, elseBlock, elseCondition);
                                break;
                            case BoundNodeKind.ReturnStatement:
                                Connect(current, _end);
                                break;
                            case BoundNodeKind.VariableDeclaration:
                            case BoundNodeKind.LabelStatement:
                            case BoundNodeKind.ExpressionStatement:
                                if (isLastStatementInBlock)
                                    Connect(current, next);
                                break;
                            default:
                                throw new Exception($"Unexpected statement: {statement.Kind}");
                        }
                    }
                }

                ScanAgain:
                foreach(var block in blocks)
                {
                    if (!block.Incoming.Any())
                    {
                        RemoveBlock(blocks, block);
                        goto ScanAgain;
                    }

                }

                blocks.Insert(0, _start);
                blocks.Add(_end);

                return new ControlFlowGraph(_start, _end, blocks, new List<BasicBlockBranch>());
            }

            private void Connect(BasicBlock from, BasicBlock to, BoundExpression condition = null)
            {
                if(condition is BoundLiteralExpression l)
                {
                    var value = (bool)l.Value;
                    if (value)
                        condition = null;
                    else
                        return;
                }

                var branch = new BasicBlockBranch(from, to, null);
                from.Outgoing.Add(branch);
                to.Incoming.Add(branch);
                _branches.Add(branch);
            }

            private void RemoveBlock(List<BasicBlock> blocks, BasicBlock block)
            {
                blocks.Remove(block);
                foreach (var branch in block.Incoming)
                {
                    branch.From.Outgoing.Remove(branch);
                    _branches.Remove(branch);
                }
            
                foreach (var branch in block.Outgoing)
                    branch.To.Incoming.Remove(branch);
            }

            private BoundExpression Negate(BoundExpression condition)
            {
                if (condition is BoundLiteralExpression literal)
                {
                    var value = (bool)literal.Value;
                    return new BoundLiteralExpression(!value);
                }

                var unaryOperator = BoundUnaryOperator.Bind(SyntaxKind.NotToken, TypeSymbol.Bool);
                return new BoundUnaryExpression(unaryOperator, condition);
            }
        }

        public void WriteTo(TextWriter writer)
        {
            string Quote(string text)
            {
                return "\"" + text.Replace("\"", "\\\"") + "\"";
            }

            writer.WriteLine("digraph G {");

            var blockIds = new Dictionary<BasicBlock, string>();
            for(int i = 0; i < Blocks.Count; i++)
            {
                var id = $"N{i}";
                blockIds.Add(Blocks[i], id);
            }

            foreach(var block in Blocks)
            {
                var id = blockIds[block];
                var label = Quote(block.ToString().Replace(Environment.NewLine, "\\l"));
                writer.WriteLine($"    {id} [label = {label} shape = box]");
            }          
            
            foreach(var branch in Branches)
            {
                var fromId = blockIds[branch.From];
                var toId = blockIds[branch.To];
                var label = Quote(branch.ToString());
                writer.WriteLine($"    {fromId} -> {toId} [label = {label}]");
            }

            writer.WriteLine("}");
        }

        public static ControlFlowGraph Create(BoundBlockStatement body)
        {
            var basicBlockBuilder = new BasicBlockBuilder();
            var blocks = basicBlockBuilder.Build(body);
            var graphBuilder = new GraphBuilder();

            return graphBuilder.Build(blocks);
        }

        public static bool AllPathsReturn(BoundBlockStatement body)
        {
            var graph = Create(body);

            foreach (var branch in graph.End.Incoming)
            {
                foreach (var statement in branch.From.Statements)
                {
                    var lastStatment = branch.From.Statements.Last();
                    if(lastStatment.Kind != BoundNodeKind.ReturnStatement)
                        return false;
                }
            }

            return true;
        }
    }
}
