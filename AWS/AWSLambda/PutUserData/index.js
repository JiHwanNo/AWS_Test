'use strict'
const AWS = require('aws-sdk')

AWS.config.update({ region : "ap-northeast-2"})

exports.handler = async (event, context, callback) =>
{
    const ddb = new AWS.DynamoDB({apiVersion: "2012-10-08"})
    const documentClinent = new AWS.DynamoDB.DocumentClient({region : "ap-northeast-2"})

    const params = 
    {
        TableName:"Study_DB",
        Item:
        {
            id: "11111",
            firstname:"No",
            lastname: "JiHwan"
        }
    }

    try
    {
        const data = await documentClinent.put(params).promise()
        console.log(data)
    }
    catch(err){console.log(err)}

}

