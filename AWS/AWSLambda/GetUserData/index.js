// 'use strict'
// const AWS = require('aws-sdk');

// AWS.config.update({ region: "ap-northeast-2"});

// exports.handler = async (event, context) => {
//   const ddb = new AWS.DynamoDB({ apiVersion: "2012-10-08"});
//   const documentClient = new AWS.DynamoDB.DocumentClient({ region: "ap-northeast-2"});

  
//   const params = {
//     TableName: "Study_DB",
//     Key: {
//       id: event.id
//     }
//   }

//   try {
//     const data = await documentClient.get(params).promise();
//     console.log(data);
//     return data;
//   } catch (err) {
//     console.log(err);
//   }
// }

//쿼리 Node.js
const AWS = require('aws-sdk')
const dynamodb = new AWS.DynamoDB.DocumentClient()

exports.handler = async (event) => {
    let response
    
    if (!event.queryStringParameters || !event.queryStringParameters.id) {
        response = {
            statusCode: 400,
            body: JSON.stringify("id가 없습니다."),
        }
        return response
    } else {
        let params = {
            Item: {
                id: event.queryStringParameters.id,
                data: event.queryStringParameters
            },
            TableName: "api_test_table",
        }
        
        await dynamodb.put(params).promise().catch(e => {
            response = {
                statusCode: 500,
                body: JSON.stringify("에러가 발생하였습니다: " + e),
            }
            return response
        })
        
        response = {
            statusCode: 200,
            body: JSON.stringify("데이터가 성공적으로 저장되었습니다.")
        }
        return response
    }
};