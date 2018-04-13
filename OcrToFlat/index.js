module.exports = function (context) {
    context.log('OcrToFlat triggered');
    let json = context.bindings.myBlob;
    let output = "";
    json.regions.forEach(region => {
        const regionText = region.lines.reduce((paragraph, line) => {
            const words = line.words.reduce((phrase, word) => `${phrase} ${word.text}`, "");
            return `${paragraph} ${words}`;
        }, "");
        //context.log(regionText);
        //context.log("\n\n");
        output = output+regionText;
        output = output+"\n";
    });
    //context.log("Output: ",output);
    context.bindings.myOutputBlob = output;
    context.log('OcrToFlat completed');
    context.done();
};