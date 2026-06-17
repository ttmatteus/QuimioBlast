using System.Collections.Generic;

namespace Minigames.AVL
{
    public class AVLNode
    {
        public int Value;
        public AVLNode Left;
        public AVLNode Right;

        public AVLNode(int value) { Value = value; }

        public int Height()
        {
            int l = Left  != null ? Left.Height()  : 0;
            int r = Right != null ? Right.Height() : 0;
            return 1 + (l > r ? l : r);
        }

        public int BalanceFactor()
        {
            int l = Left  != null ? Left.Height()  : 0;
            int r = Right != null ? Right.Height() : 0;
            return l - r;
        }

        public bool IsBalanced()
        {
            int bf = BalanceFactor();
            if (bf < -1 || bf > 1) return false;
            bool leftOk  = Left  == null || Left.IsBalanced();
            bool rightOk = Right == null || Right.IsBalanced();
            return leftOk && rightOk;
        }
    }

    public class AVLTree
    {
        public AVLNode Root { get; private set; }

        // Insere sem balanceamento automático — cria cenário desbalanceado para o jogador resolver.
        public void InsertUnbalanced(int value)
        {
            Root = InsertBST(Root, value);
        }

        private AVLNode InsertBST(AVLNode node, int value)
        {
            if (node == null) return new AVLNode(value);
            if (value < node.Value) node.Left  = InsertBST(node.Left,  value);
            else                    node.Right = InsertBST(node.Right, value);
            return node;
        }

        // Gera o cenário inicial fixo: 30→20→10 (cadeia à esquerda — desbalanço LL)
        // Estrutura resultante:
        //        30
        //       /
        //      20
        //     /
        //    10
        public void BuildUnbalancedScenario()
        {
            Root = null;
            InsertUnbalanced(30);
            InsertUnbalanced(20);
            InsertUnbalanced(10);
        }

        // ── Rotações estruturais ──────────────────────────────────────────────

        // Rotação Direita (LL fix): retorna nova raiz local
        public AVLNode RotateRight(AVLNode y)
        {
            AVLNode x  = y.Left;
            AVLNode T2 = x.Right;
            x.Right = y;
            y.Left  = T2;
            return x;
        }

        // Rotação Esquerda (RR fix): retorna nova raiz local
        public AVLNode RotateLeft(AVLNode x)
        {
            AVLNode y  = x.Right;
            AVLNode T2 = y.Left;
            y.Left  = x;
            x.Right = T2;
            return y;
        }

        // Dupla Esquerda-Direita (LR fix)
        public AVLNode RotateLeftRight(AVLNode node)
        {
            node.Left = RotateLeft(node.Left);
            return RotateRight(node);
        }

        // Dupla Direita-Esquerda (RL fix)
        public AVLNode RotateRightLeft(AVLNode node)
        {
            node.Right = RotateRight(node.Right);
            return RotateLeft(node);
        }

        // Aplica uma rotação identificada pelo enum no nó com o valor informado.
        // Retorna true se a rotação foi aplicada com sucesso.
        public bool ApplyRotation(int targetValue, RotationType type)
        {
            bool applied = false;
            Root = ApplyRotationRecursive(Root, targetValue, type, ref applied);
            return applied;
        }

        private AVLNode ApplyRotationRecursive(AVLNode node, int target, RotationType type, ref bool applied)
        {
            if (node == null) return null;

            if (node.Value == target)
            {
                AVLNode result = ExecuteRotation(node, type);
                if (result != null) { applied = true; return result; }
                return node;
            }

            node.Left  = ApplyRotationRecursive(node.Left,  target, type, ref applied);
            node.Right = ApplyRotationRecursive(node.Right, target, type, ref applied);
            return node;
        }

        private AVLNode ExecuteRotation(AVLNode node, RotationType type)
        {
            switch (type)
            {
                case RotationType.Right:      return node.Left  != null ? RotateRight(node)      : null;
                case RotationType.Left:       return node.Right != null ? RotateLeft(node)        : null;
                case RotationType.LeftRight:  return node.Left  != null && node.Left.Right != null  ? RotateLeftRight(node)  : null;
                case RotationType.RightLeft:  return node.Right != null && node.Right.Left != null  ? RotateRightLeft(node)  : null;
                default: return null;
            }
        }

        public bool IsFullyBalanced() => Root == null || Root.IsBalanced();

        // Retorna todos os nós em ordem (BFS) para mapeamento de UI.
        public List<AVLNode> GetAllNodesBFS()
        {
            var result = new List<AVLNode>();
            if (Root == null) return result;
            var queue = new Queue<AVLNode>();
            queue.Enqueue(Root);
            while (queue.Count > 0)
            {
                AVLNode current = queue.Dequeue();
                result.Add(current);
                if (current.Left  != null) queue.Enqueue(current.Left);
                if (current.Right != null) queue.Enqueue(current.Right);
            }
            return result;
        }

        public AVLNode FindNode(int value)
        {
            return FindRecursive(Root, value);
        }

        private AVLNode FindRecursive(AVLNode node, int value)
        {
            if (node == null) return null;
            if (node.Value == value) return node;
            AVLNode left  = FindRecursive(node.Left,  value);
            return left != null ? left : FindRecursive(node.Right, value);
        }
    }

    public enum RotationType
    {
        Left,
        Right,
        LeftRight,
        RightLeft
    }
}
