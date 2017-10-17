module Domain

open System

type Entry = {
    tenantHash:string
    message:string
    tag:string
    created:DateTime
}