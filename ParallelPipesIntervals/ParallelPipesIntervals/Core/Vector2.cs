namespace ParallelPipesIntervals.Core
{
    public struct Vector2<T, Q>
    {
        private T _x;
        private Q _y;

        public T X => _x;
        public Q Y => _y;

        public Vector2(T x, Q y)
        {
            this._x = x;
            this._y = y;
        }
    }
}