namespace Entity.Enemy.FSM {
    public interface IFindPlayer<TTarget> {
        public bool PlayerExist(TTarget pTarget);
    }
}