const fs = require('fs');
const readline = require('readline'); 
async function readFile(filepath) {
    try{
        const rl = readline.createInterface({
            input: fs.createReadStream(filepath),
            crlfDelay: Infinity
        });

        for await(const line of rl){
            let parts = line.split(";");
            let A = parts[0], B = parts[1];
            console.log(`INSERT INTO dantoc(TenDanToc,TenGoiKhac) VALUES("${A}","${B}");`);
        }
    }catch(err){
        console.error('Erro reading file: ',err);
    }
}
readFile('datatemp.txt');