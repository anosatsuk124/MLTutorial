# /usr/bin/env bash

cd assets

wget https://archive.ics.uci.edu/ml/machine-learning-databases/00331/sentiment%20labelled%20sentences.zip

unzip "sentiment labelled sentences.zip"

if [ ! -d ../Data/ ]; then
    mkdir ../Data
fi

cp "./sentiment labelled sentences/yelp_labelled.txt" ../Data