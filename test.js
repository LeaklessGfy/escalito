class Obj {
  callback = null;
  state = 0;

  async() {
    return new Promise((resolve, reject) => {
      this.callback = resolve;
      this.state = 1;
      // other stuff
    });
  }

  later() {
    if (this.callback !== null) {
      this.callback();
      this.clean();
      console.log('end');
    }
  }

  clean() {
    this.state = 0;
  }

  async setStateTo1() {
    this.state = 1;
  }

  doSomethingIfState1() {
    console.log('do');
    if (this.state === 1) {
      console.log('here');
    }
  }
}

async function func(o) {
  await o.async();
  await o.setStateTo1();
  o.doSomethingIfState1();
}

const o = new Obj();
func(o);
o.later();
