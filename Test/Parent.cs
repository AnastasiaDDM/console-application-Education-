﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class Parent
    {
        public int ID { get; set; }
        public string FIO { get; set; }
        public string Phone { get; set; }
        public Nullable<System.DateTime> Deldate { get; set; }
        public Nullable<System.DateTime> Editdate { get; set; }

        public Parent()
        {
        }
        public Parent ParentID(int id)
        {
            using (SampleContext db = new SampleContext())
            {
                Parent v = db.Parents.Where(x => x.ID == id).FirstOrDefault<Parent>();

                return v;
            }
        }

        public static List<Student> GetStudents(int id)   // Получение списка учеников этого родителя
        {
            List<Student> liststudents = new List<Student>();
            using (SampleContext db = new SampleContext())
            {
                var students = from p in db.Parents
                               join sp in db.StudentsParents on p.ID equals sp.ParentID
                               join s in db.Students on sp.StudentID equals s.ID
                               select new { SID = s.ID, SPhone = s.Phone, SFIO = s.FIO, SDelDate = s.Deldate, PID = p.ID, ParID = sp.ParentID, StID = sp.StudentID };

                //             Parent par = ParentID(); запрос на получение текущего родителя (ParentID) будет проходить из формы !!!!!!! 
                students = students.Where(x => x.ParID == id );
                students = students.Where(x => x.SID == x.StID);

                foreach (var p in students)
                {
                    liststudents.Add(new Student { ID = p.SID, Phone = p.SPhone, Deldate = p.SDelDate, FIO = p.SFIO });  // Добавление учеников в список
                }

                return liststudents;
            }
        }
    }
        public class Parents
    {
        public static List<Parent> GetPa(SampleContext context)
        {
            //      var context = new SampleContext();

            var parents = context.Parents.ToList();
            return parents;
        }


        //////////////////// ОДИН БОЛЬШОЙ ПОИСК !!! Если не введены никакие параметры, функция должна возвращать всех родителей //////////////////
        public static List<Parent> FindAll(Boolean deldate,Parent parent, Student student, String sort, String askdesk, int page, int count) //deldate =false - все и удал и неудал!
        {
            List<Parent> parentList = new List<Parent>();

            using (SampleContext db = new SampleContext())
            {
                // Соединение необходимых таблиц
                var parents = from p in db.Parents
                join sp in db.StudentsParents on p.ID equals sp.ParentID
                join s in db.Students on sp.StudentID equals s.ID
                              select new { PID = p.ID, PPhone = p.Phone, PFIO = p.FIO, PDelDate = p.Deldate, SPhone = s.Phone, SFIO = s.FIO, SID = s.ID };


                    // Последовательно просеиваем наш список

                if (deldate != false) // Убираем удаленных, если нужно
                {
                    parents = parents.Where(x => x.PDelDate == null);
                }

                if (student.Phone != null)
                {
                    parents = parents.Where(x => x.SPhone == student.Phone);
                }

                if (student.FIO != null)
                {
                    parents = parents.Where(x => x.SFIO == student.FIO);
                }

                if (student.ID != 0)
                {
                    parents = parents.Where(x => x.SID == student.ID);
                }

                if (parent.FIO != null)
                {
                    parents = parents.Where(x => x.PFIO == parent.FIO);
                }

                if (parent.Phone != null)
                {
                    parents = parents.Where(x => x.PPhone == parent.Phone);
                }

                if (sort != null)  // Сортировка, если нужно
                {
                    if (askdesk == "desk")
                    {
                        parents = parents.OrderByDescending(u => sort);
                    }
                    else
                    {
                        parents = parents.OrderBy(u => sort);
                    }
                }
                else { parents = parents.OrderBy(u => u.PID);  }

                parents = parents.Skip((page-1) * count).Take(count);  // Формирование страниц и кол-во записей на странице

                foreach (var p in parents) 
                {
                    if (parentList.Find(x => x.ID == p.PID) == null)
                    {
                        parentList.Add(new Parent { ID = p.PID, Phone = p.PPhone, Deldate = p.PDelDate, FIO = p.PFIO }); // Добавление родителя в лист, если такого еще нет, это для предохранения от дубликатов
                    }
                }
                          
                return parentList;
            }
        }
    }
}

