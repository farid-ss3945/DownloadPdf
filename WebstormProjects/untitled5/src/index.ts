// task 1
/*let a: number = Number(prompt("number:"));
alert(a * 1024 / (820 / 8));*/


// task 2
/*
let b: string = prompt('number: ')!;
let num = Number(b);
alert(num % 10 * 100 + (((num % 100) - (num % 10)) / 10) * 10 + ((num - (num % 100)) / 100));
*/


// task 3
/*
let n: number = Number(prompt('number: '));
(n % 2 === 0) && alert("even");
(n % 2 !== 0) && alert("odd");
*/


// task 4
/*
let age: number = Number(prompt("Age:"));

if (age >= 0 && age < 12) {
    alert("lil bro");
} else if (age >= 12 && age < 18) {
    alert("teen");
} else if (age >= 18 && age < 60) {
    alert("unc");
} else if (age >= 60) {
    alert("grave");
} else {
    alert("incorrect age");
}
*/


// task 5
/*
let n: number = Number(prompt("0 to 9: "));
let symbol: string;

switch (n) {
    case 1: symbol = "!"; break;
    case 2: symbol = "@"; break;
    case 3: symbol = "#"; break;
    case 4: symbol = "$"; break;
    case 5: symbol = "%"; break;
    case 6: symbol = "^"; break;
    case 7: symbol = "&"; break;
    case 8: symbol = "*"; break;
    case 9: symbol = "(" ; break;
    case 0: symbol = ")" ; break;
    default: symbol = "incorrect symbol."; break;
}

alert(symbol);
*/


// task 6
/*
let year: number = Number(prompt("year:"));

if (year % 400 === 0) {
    alert("leap");
} else if (year % 4 === 0 && year % 100 !== 0) {
    alert("leap");
} else {
    alert("not leap");
}
*/


// task 7
/*
let positive = 0;
let negative = 0;
let zeros = 0;
let even = 0;
let odd = 0;

for (let i = 0; i < 10; i++) {
    const num: number = Number(prompt(`Enter number ${i + 1} of 10:`));

    if (num > 0) positive++;
    else if (num < 0) negative++;
    else zeros++;

    if (num % 2 === 0) even++;
    else odd++;
}

alert(
    "Positive: " + positive +
    "\nNegative: " + negative +
    "\nZeros: " + zeros +
    "\nEven: " + even +
    "\nOdd: " + odd
);
*/


// task 8
/*
while (true) {
    let expres: string = prompt("expression:")!;
    alert(eval(expres) + "\nanother one?");
    let dec: string = prompt("continue?")!;

    if (dec.toLowerCase() === "no") break;
}
*/


// task 9
/*
function comp(num_1: number, num_2: number): number {
    if (num_1 > num_2) return 1;
    else if (num_1 < num_2) return -1;
    return 0;
}

let a: number = Number(prompt()!);
let b: number = Number(prompt()!);

alert(comp(a, b));
*/


// task 10
/*
const num: number = Number(prompt());

function factorial(n: number): number {
    if (n <= 1) return 1;
    return n * factorial(n - 1);
}

alert(factorial(num));
*/


// task 11
/*
function sec(h: number, m: number, s: number): number {
    return h * 3600 + m * 60 + s;
}

let _h: number = Number(prompt());
let _m: number = Number(prompt());
let _s: number = Number(prompt());

alert(sec(_h, _m, _s));
*/


// task 12
/*
function f(t: number): void {
    let h = Math.floor(t / 3600);
    let m = Math.floor((t % 3600) / 60);
    let s = t % 60;

    alert(`Hours: ${h}\nMinutes: ${m}\nSeconds: ${s}`);
}

let a: number = Number(prompt());
f(a);
*/


// task 13
/*
function sum(...args: number[]): number {
    let total = 0;
    for (let i = 0; i < args.length; i++) {
        total += args[i];
    }
    return total;
}

console.log(sum(1, 2, 3, 4, 5, 6));
*/