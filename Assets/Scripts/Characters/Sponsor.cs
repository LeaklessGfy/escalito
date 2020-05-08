namespace Characters
{
    public class Sponsor : Character
    {
        private void OnMouseDown()
        {
            print("Hello");
        }
        
        protected override bool Flip(float x)
        {
            return x > transform.position.x;
        }
    }
}